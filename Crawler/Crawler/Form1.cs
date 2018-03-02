using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Data.SqlClient;


using Abot.Crawler;
using Abot.Poco;
using System.Text.RegularExpressions;


namespace Crawler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DBmanager db = new DBmanager(); 

        /* httpServicePointConnectionLimit="200"  
      httpRequestTimeoutInSeconds="15" 
      httpRequestMaxAutoRedirects="7" 
      isHttpRequestAutoRedirectsEnabled="true" 
      isHttpRequestAutomaticDecompressionEnabled="false"
      isSendingCookiesEnabled="false"
      isSslCertificateValidationEnabled="false"
      isRespectUrlNamedAnchorOrHashbangEnabled="false"
      minAvailableMemoryRequiredInMb="0"
      maxMemoryUsageInMb="0"
      maxMemoryUsageCacheTimeInSeconds="0"
      maxCrawlDepth="1000"
      maxLinksPerPage="1000"
      isForcedLinkParsingEnabled="false"
      maxRetryCount="0"
      minRetryDelayInMilliseconds="0"
      />
    <authorization
      isAlwaysLogin="false"
      loginUser=""
      loginPassword="" />     
    <politeness 
      isRespectRobotsDotTextEnabled="false"
      isRespectMetaRobotsNoFollowEnabled="false"
      isRespectHttpXRobotsTagHeaderNoFollowEnabled="false"
      isRespectAnchorRelNoFollowEnabled="false"
      isIgnoreRobotsDotTextIfRootDisallowedEnabled="false"
      robotsDotTextUserAgentString="abot"
      maxRobotsDotTextCrawlDelayInSeconds="5" 
      minCrawlDelayPerDomainMilliSeconds="0"/>*/


        List<string> links = new List<string>(); 


        void testWebAbot()
        {
            CrawlConfiguration crawlConfig = new CrawlConfiguration();
            crawlConfig.CrawlTimeoutSeconds = 100;
            crawlConfig.MaxConcurrentThreads = 50;
            crawlConfig.MaxPagesToCrawl = 3000;
            crawlConfig.UserAgentString = "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko";
            crawlConfig.ConfigurationExtensions.Add("SomeCustomConfigValue1", "1111");
            crawlConfig.ConfigurationExtensions.Add("SomeCustomConfigValue2", "2222");
            crawlConfig.IsSendingCookiesEnabled = false;
            crawlConfig.HttpServicePointConnectionLimit = 200;  
            crawlConfig.MaxLinksPerPage = 3000;
            crawlConfig.MaxCrawlDepth = 10000;
            crawlConfig.IsExternalPageCrawlingEnabled = true; 
            
            crawlConfig.MaxPagesToCrawlPerDomain = 10000;

            crawlConfig.MaxLinksPerPage = 0;
            crawlConfig.IsHttpRequestAutoRedirectsEnabled = true;
            crawlConfig.MaxMemoryUsageInMb = 0; 

            
              var  crawler = new PoliteWebCrawler(crawlConfig);
           
           crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
           crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
           crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
           crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;
       
            CrawlResult result = crawler.Crawl(new Uri(textBox1.Text)); //This is synchronous, it will not go to the next line until the crawl has completed

            if (result.ErrorOccurred)
                Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
            else
                Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);
        }

        private void syncPageCrawledCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
            {
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);
                links.Add(crawledPage.Uri.AbsoluteUri);
                listBox1.DataSource = links;
            }

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);

            var htmlAgilityPackDocument = crawledPage.HtmlDocument; //Html Agility Pack parser

        
            var angleSharpHtmlDocument = crawledPage.AngleSharpHtmlDocument; //


        }





        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            links.Add(pageToCrawl.Uri.AbsoluteUri);
            listBox1.DataSource = links;
    
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
            { 
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);
                links.Add(crawledPage.Uri.AbsoluteUri);
                listBox1.DataSource = links;
            }

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);

            var htmlAgilityPackDocument = crawledPage.HtmlDocument; //Html Agility Pack parser
            var angleSharpHtmlDocument = crawledPage.AngleSharpHtmlDocument; //AngleSharp parser


            var nodes = htmlAgilityPackDocument.DocumentNode.SelectNodes("//body//text()");
            string text = "";
            foreach (var item in nodes)
            {
                text += item.InnerText+" " ;
            }

    var pageTitle =        htmlAgilityPackDocument.DocumentNode.SelectSingleNode("//title").InnerText;
    var pageImage = htmlAgilityPackDocument.DocumentNode.SelectSingleNode("//img").Attributes["src"] ; 
  
            var trueContent  = Regex.Replace(text, @"\t|\n|\r", "");
            int AAA = 0;    
           
       //     db.insertLinks(e.CrawledPage.Uri.AbsoluteUri, trueContent);
            //   Console.WriteLine(text);
 
           
        }


        void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }



       async Task downloadWebPag( Uri url )
        {
            try
            {
                WebRequest myWebRequest;
                WebResponse myWebResponse;
                myWebRequest = WebRequest.Create(url);
                myWebResponse = myWebRequest.GetResponse();
                if (((HttpWebResponse)myWebResponse).StatusCode == HttpStatusCode.OK)
                {
                    Stream streamResponse = myWebResponse.GetResponseStream();
                    StreamReader sReader = new StreamReader(streamResponse);
                    String rString = sReader.ReadToEnd();

                    String l = returnLinks(rString ,  url);
                    textBox2.Text = rString;

                    streamResponse.Close();
                    sReader.Close();
                    myWebResponse.Close();
                }
                else
                {
                    Console.WriteLine("error downloading HTML "); 
                }

            }
            catch (Exception e)
            {
                throw e; 
            }
            await Task.Yield(); 
           

        }


        

       Queue<Uri> urls = new Queue<Uri>();
       HashSet<string> visited = new HashSet<string>(); 
      Dictionary< Uri , Uri> parentList = new Dictionary<Uri,Uri>(); 
        private void button1_Click(object sender, EventArgs e)
        {
            testWebAbot(); 
           // returnLinks("sad"); 
          /*  listBox1.Items.Clear();

            urls.Enqueue(new Uri(textBox1.Text));

            Task.Run(() => Docrawl()); */

        }





        async Task  Docrawl()
        {

            while ( urls.Count > 0  )
            
            {
                var currUrl = urls.Dequeue(); 
                
              await  Task.Run(() => downloadWebPag(currUrl ));

            }
         await    Task.Yield(); 
        }


        async Task SaveToDB()
        {
            foreach (var item in visited)
            {
                db.insertLinks(item,"");
            }
            await Task.Yield();
        }

        private String returnLinks(String r,Uri curr )
        {
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

            // There are various options, set as needed
            htmlDoc.OptionFixNestedTags = true;

            // filePath is a path to a file containing the html
            htmlDoc.LoadHtml(r);

            // Use:  htmlDoc.LoadHtml(xmlString);  to load from a string (was htmlDoc.LoadXML(xmlString)

            // ParseErrors is an ArrayList containing any errors from the Load statement
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required

            }
            else
            {

                if (htmlDoc.DocumentNode != null)
                {

                    var a = htmlDoc.DocumentNode.SelectNodes("//a//@href");

                    if (a.Count == 0)
                        listBox1.Items.Add("no links !! ");
                     var nodes = htmlDoc.DocumentNode.SelectNodes("//body//text()");
            string text = "";
            foreach (var item in nodes)
            {
                text += item.InnerText+" " ;
            }

            var trueContent  = Regex.Replace(text, @"\t|\n|\r", "");
            int AAA = 0;

            db.insertLinks(curr.AbsoluteUri, trueContent);
 
                    foreach (HtmlAgilityPack.HtmlNode item in a)
                    {
                        string CurrLink = item.Attributes["href"].Value;
                        Console.WriteLine(CurrLink); 

                        if (!visited.Contains(CurrLink))
                        {
                            try
                            {
                                var genUrl = new Uri(CurrLink);

                                parentList.Add(genUrl, curr);
                                urls.Enqueue(genUrl);
                                visited.Add(CurrLink);
                              
                                listBox1.Items.Add(item.InnerText + " " + CurrLink);

                            }

                            catch (Exception e)
                            {
                                try
                                {
                                    //var genUrl = new Uri(aa);
                                    //Uri concatenated = new Uri(parentList[genUrl].OriginalString + genUrl.OriginalString);
                                    //Console.WriteLine("concated url : " + concatenated.OriginalString);
                                    //urls.Enqueue(genUrl);
                                    //visited.Add(concatenated.OriginalString);

                                  //  Console.WriteLine(e.Message);
                                }
                                catch (Exception ee)
                                {
                                   // Console.WriteLine(ee.Message);
                                }
                            }
                        }

                    }
                }
                else
                {
                    Console.WriteLine("null expection .");
                }
            }
            
            String s = "";

          
            return s;
        }

        private void button2_Click(object sender, EventArgs e)
        {


            DBform dbf = new DBform();
            dbf.Show();
          
  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Task.Run(()=> SaveToDB());
        }
      
    








    }
}
