using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Text;


namespace SearchEngine
{
    public partial class SearchForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsCallback)
            {
                ConnectToDB();


              //  TestDeuxAlgo(); 
          //       intersectTest(); 

                // var x =  getURLOF(new List<int>() { 2, 1, 3, 5 }); 

                // var xx = getDocIdsOF("main"); 


                //  var xxx = Search_multi(" that main "); 

            //    test_spelling();

        //    var xx =    getPositionalIndexDic("forget");


         
                
               // var xx =        getListOfWordsFromDB("S150");
            }
        }

        #region CheckSpelling
        //Calculate the edit distance
        private static int CalcLevenshteinDistance(string a, string b)
        {
            if (String.IsNullOrEmpty(a) || String.IsNullOrEmpty(b)) return 0;

            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }
            return distances[lengthA, lengthB];
        }

        //get bigrams of a word
        private List<string> get_bigrams(string word)
        {
            List<string> bigrams = new List<string>();
            string sub = "";
            for (int i = 0; i < word.Length - 1; i++)
            {
                sub = word.Substring(i, 2);
                bigrams.Add(sub);
            }

            return bigrams;
        }
        //check spelling 
        private List<string> related_words(string query, Dictionary<string, List<string>> words)
        {
            List<string> related_words = new List<string>();

            List<string> query_bigrams = get_bigrams(query);
            int query_bigrams_count = query_bigrams.Count();
            int common_bigrams = 0;

            foreach (KeyValuePair<string, List<string>> entry in words)
            {
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    for (int k = 0; k < query_bigrams.Count; k++)
                    {
                        if (query_bigrams[k].Equals(entry.Value[i]))
                            common_bigrams++;
                    }

                }
                common_bigrams *= 2;
                float temp = query_bigrams_count + entry.Value.Count;
                float applicable = common_bigrams / temp;
                if (applicable > 0.45)
                {
                    related_words.Add(entry.Key);
                }
                common_bigrams = 0;
            }


            return related_words;
        }

        void test_spelling()
        {
            Dictionary<string, List<string>> words = new Dictionary<string, List<string>>();
            words["aboard"] = get_bigrams("aboard");
            words["lord"] = get_bigrams("lord");
            words["border"] = get_bigrams("border");
            words["ardent"] = get_bigrams("ardent");
            words["any"] = get_bigrams("any");
            words["rdrdrd"] = get_bigrams("rdrdrdr");
            words["blabla"] = get_bigrams("blabla");
            words["tamatmya"] = get_bigrams("tamatmya");
            words["tamatmya"] = get_bigrams("otaya");
            words["rozya"] = get_bigrams("rozya");
            List<string> test = related_words("bord", words);
        }
        #endregion 

        void TestDeuxAlgo()
        {
            Dictionary<string , int > map = new Dictionary<string,int>(); 
            map["fools"]= 1; 
            map["rush"] =2 ; 
            map["in"] = 3 ;

             List<int> searchWord = new List<int>(){1,2,3}  ;
            Dictionary < string , Dictionary <int , List<int >>> positionalIndex = new Dictionary<string,Dictionary<int,List<int>>>();
            positionalIndex["fools"] = new Dictionary<int, List<int>>();
            positionalIndex["in"] = new Dictionary<int, List<int>>();
            positionalIndex["rush"] = new Dictionary<int, List<int>>(); 

            positionalIndex["fools"][2] = new List<int>() { 1, 17, 74, 222 };
            positionalIndex["fools"][4] = new List<int>() { 8, 78, 108, 458 };
            positionalIndex["fools"][7] = new List<int>() { 3, 13, 23, 193 };
            positionalIndex["in"][2] = new List<int>() { 3,37,76,444,851};
            positionalIndex["in"][4] = new List<int>() { 10, 20, 110, 470, 500 };
            positionalIndex["in"][7] = new List<int>() { 5, 15, 25, 195 };
            positionalIndex["rush"][2] = new List<int>() { 2, 66, 194, 321, 702 }; 
             positionalIndex["rush"][4] =  new List<int>(){9,69,149,429,569};
             positionalIndex["rush"][7] = new List<int>() { 4, 14, 404 } ;


            var docs =  specialDeuxAlgo(searchWord, map, positionalIndex); 

            //correct output is  2  4 7 
            // out is 2 4 7  (Y_ Y) ♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪♪
        }
        void ConnectToDB()
        {
            connection = new MySqlConnection(ConStr);

        }

        static string ConStr = "server=localhost;user=root;database=crawler;port=3306;";
        MySqlConnection connection;
        stemclass Stemmer = new stemclass();

        List<Uri> Search_multi(string search_word)
        {

            List<Uri> links = new List<Uri>();
            var splited = search_word.Split(' ');
            Dictionary<string, List<int>> whereEachWord = new Dictionary<string, List<int>>();
            foreach (var item in splited)
            {
                //english 
                if (Regex.IsMatch(item, @"^[a-zA-Z|]+$"))
                {
                    var Stemmed = Stemmer.stem(item);
                    List<int> docsIdConatinItem = getDocIdsOF(item);
                    whereEachWord.Add(Stemmed, docsIdConatinItem);
                }
            }

            List<int> intersectedIds = Intersect(whereEachWord);
            List<string> intersectedIdsString = getURLOF(intersectedIds);
            foreach (var item in intersectedIdsString)
            {
                links.Add(new Uri(item));
            }

            return links;
        }

        List<Uri> Search_Exact(string search_word)
        {

            List<Uri> links = new List<Uri>();
            var splited = search_word.Split(' ');
            Dictionary<string, List<int>> whereEachWord = new Dictionary<string, List<int>>();
            Dictionary<string, int> mappedKeys = new Dictionary<string, int>();
            List<int> MappedSearchKey = new List<int>(); 
            int i = 1;
            foreach (var item in splited)
            {
                //english 
                if (Regex.IsMatch(item, @"^[a-zA-Z|]+$"))
                {
                    var Stemmed = Stemmer.stem(item);
                    List<int> docsIdConatinItem = getDocIdsOF(item);
                    whereEachWord.Add(Stemmed, docsIdConatinItem);
                    mappedKeys.Add(Stemmed, i++);
                    MappedSearchKey.Add(mappedKeys[Stemmed]); 
                }

            }

          

            List<int> intersectedIds = Intersect(whereEachWord);

            Dictionary<string, Dictionary<int, List<int>>> positionalIndex = new Dictionary<string, Dictionary<int, List<int>>>();
            foreach (var item in mappedKeys.Keys)
            {
                positionalIndex.Add(item, getPositionalIndexDic(item));
            }


            //filter only the intersected ones 
            Dictionary<string, Dictionary<int, List<int>>> positionalIndexTmp = new Dictionary<string, Dictionary<int, List<int>>>();
            foreach (var term1 in positionalIndex.Keys)
            {
                Dictionary<int, List<int>> newDic = new Dictionary<int, List<int>>();
                foreach (var item in intersectedIds)
                {
                    if (!newDic.ContainsKey(item))
                    {
                        if (positionalIndex.ContainsKey(term1))
                        { 
                            if ( positionalIndex[term1].ContainsKey(item))
                            newDic.Add(item, positionalIndex[term1][item]);
                        }
                    }
                }

                positionalIndexTmp.Add(term1, newDic);

            }

            var docsHaveExact = specialDeuxAlgo(MappedSearchKey, mappedKeys, positionalIndexTmp); ;
            List<string> intersectedIdsString = getURLOF(docsHaveExact);

            foreach (var item in intersectedIdsString)
            {
                links.Add(new Uri(item));
            }

            return links;


        }



        List<int> getPositionsofTermInDoc(string term, int docid, Dictionary<string, Dictionary<int, List<int>>> referenc)
        {
            return referenc[term][docid];
        }
        private List<int> specialDeuxAlgo(List<int >  MappedSearchKey ,  Dictionary<string, int> mappedKeys , Dictionary<string, Dictionary<int, List<int>>> positionalIndexTmp)
        {

            List<int > WantedIDS = new List<int>(); 
            var docIDs = positionalIndexTmp.First().Value.Keys;

            foreach (var id in docIDs)
            {
                SortedDictionary<int, int> merged = new SortedDictionary<int, int>();
                foreach (var term in positionalIndexTmp.Keys)
                {
                    var CurTermPos = getPositionsofTermInDoc(term, id, positionalIndexTmp);
                    var mappedValue = mappedKeys[term];

                    foreach (var pos in CurTermPos)
                    {
                        merged.Add(pos, mappedValue);
                    }
                }
                // TODO: 
                /// foreach merged find gap between numbers then insert median number with null value as barrier to solve 
                /// the problem of  main $%#$%$%#$% page which is NOT an Exact phrase 

                if (ContinousSubSet(merged.Values.ToList(),MappedSearchKey ))
                {
                    WantedIDS.Add(id); 
                } 

            }

            return WantedIDS;
        }
        private string GetPositionalIndeces(string content)
        {
            connection.Open();
            string s = "";
            MySqlCommand searchCmd = new MySqlCommand();
            searchCmd.CommandText = "SELECT distinct position from indexedTerms where Term = '" + content + "';";
            searchCmd.Connection = connection;
            var read = searchCmd.ExecuteReader();
            //int orange=0;
            while (read.Read())
            {
                s = read["Position"].ToString();
            }
            connection.Close();
            return s;
        }

        Dictionary<int, List<int>> getPositionalIndexDic(string term)
        {
            return  parse(GetPositionalIndeces(term));

        }
        private Dictionary<int, List<int>> parse(string s)
        {
            Dictionary<int, List<int>> myDictionary = new Dictionary<int, List<int>>();
            Dictionary<int, int> mydic = new Dictionary<int, int>();
            List<string> b = new List<string>();
            List<string> sp = new List<string>();
            sp = s.Split('/').ToList();
            for (int i = 0; i < sp.Count; i++)
            {
                b = sp[i].Split(',').ToList();
                List<int> f = new List<int>();
                for (int j = 0; j < b.Count; j++)
                {
                    string[] splitter = b[j].Split(':');
               //     if ( splitter.Length >1 )
                    if (splitter.Length < 2 || string.IsNullOrEmpty(splitter[1]))
                        continue; 

                    f.Add(int.Parse(splitter[1]));

                }
                string[] splite = sp[i].Split(':');

                if (splite.Length < 2 || string.IsNullOrEmpty(splite[0]))
                    continue; 

                myDictionary.Add(int.Parse(splite[0]), f);

            }
            return myDictionary;
        }



        private List<int> Intersect(Dictionary<string, List<int>> whereEachWord)
        {
            if (whereEachWord.Values.Count > 0)
            {
                List<int> intersection = whereEachWord.First().Value;

                foreach (var item in whereEachWord)
                {
                    intersection = new List<int>(intersection.Intersect(item.Value));
                }

                return intersection;
            }
            else throw new Exception("intersection dictionairy empty ");
        }


        void intersectTest()
        {
            Dictionary<string, List<int>> dic = new Dictionary<string, List<int>>();
            dic["that"] = new List<int>() { 1, 2, 5, 4, 6, 7, 6, 2 };
            dic["this"] = new List<int>() { 3, 5, 7, 8 };
            dic["me"] = new List<int>() { 5,6};

            var x = ContinousSubSet(dic["that"], dic["me"]);
            var intersected = Intersect(dic);


        }


        bool ContinousSubSet(List<int> large, List<int> small)
        {
            int j = 0; int i = 0;
            for (; i < large.Count; )
            {
                var tmp = i;
                while (j < small.Count && i < large.Count && small[j] == large[i])
                {
                    j++; i++;
                }
                if (j == small.Count)
                    return true;
                else
                {
                    j = 0; i = tmp; i++;

                }
            }

            return false;
        }
        private List<int> parseComa(string s)
        {
            List<string> ret = new List<string>();
            List<int> ret2 = new List<int>();
            ret = s.Split(',').ToList();
            foreach (var item in ret)
            {
                int xx;
                int.TryParse(item, out xx);
                ret2.Add(xx);
            }
            return ret2;
        }

        private List<string> getURLOF(List<int> item)
        {
            connection.Open();
            string commandStr = "select url from mylinks2 where  ";
            for (int i = 0; i < item.Count - 1; i++)
            {
                commandStr += "id = ";
                commandStr += item[i].ToString();
                commandStr += " or ";
            }
            commandStr += "id = ";
            commandStr += item[item.Count - 1];
            commandStr += "; ";
            MySqlCommand cmd = new MySqlCommand(commandStr, connection);
            var red = cmd.ExecuteReader();

            List<string> res = new List<string>();
            while (red.Read())
            {
                res.Add(red["url"].ToString());
            }
            connection.Close();
            return res;
        }

        private List<int> getDocIdsOF(string item)
        {
            connection.Open();
            string commandStr = "select distinct docid from indexedTerms where term =  '" + item + "' ; ";
            MySqlCommand cmd = new MySqlCommand(commandStr, connection);
            var red = cmd.ExecuteReader();
            List<string> res = new List<string>();
            while (red.Read())
            {
                res.Add(red["docid"].ToString());
            }
            connection.Close();

            if (res.Count > 0)
                return parseComa(res[0]);
            else
                throw new Exception("empty res docids ");
        }



        protected void Button1_Click(object sender, EventArgs e)
        {


            try
            {
                if (!ExactSearch.Checked)
                {
                    var xx = Search_multi(TextBox1.Text);
                    DisplyDocs(xx);
                }
                else
                {
                    var xx = Search_Exact(TextBox1.Text);
                    DisplyDocs(xx);
                }
            }
            catch (Exception ex)
            {

                PlaceHolder1.Controls.Add(new LiteralControl("Sorry There is Nothing Matching your query ! try other Words <br/> Cause : " + ex.Message));
            }

        }
        private void DisplyDocs(List<Uri> s)
        {

            for (int i = 1; i < s.Count; i++)
            {
                HyperLink hyper = new HyperLink();
                hyper.ForeColor = System.Drawing.Color.Blue;
                //  string filename = GetTitle(s[i].AbsoluteUri); 
                //  hyper.Text = GetWebPageTitle( s[i].AbsoluteUri);
                hyper.Text = s[i].ToString();
                hyper.NavigateUrl = s[i].AbsoluteUri;
                hyper.Target = "_blank";
                hyper.Font.Size = 30;

                //PlaceHolder1.Controls.Add(new Image() { Height = 200 , Width = 200 ,
                //                                        ImageUrl = "http://www.haotu.net/up/3770/128/Flappy-Bird.png"
                //});
                PlaceHolder1.Controls.Add(hyper);
                PlaceHolder1.Controls.Add(new LiteralControl("<br/>"));
                PlaceHolder1.Controls.Add(new LiteralControl("<br/>"));
                PlaceHolder1.Controls.Add(new LiteralControl("<br/>"));
                PlaceHolder1.Controls.Add(new LiteralControl("<br/>"));
                PlaceHolder1.Controls.Add(new LiteralControl("<br/>"));
                PlaceHolder1.Controls.Add(new LiteralControl("<br/>"));

            }
        }



        string GetTitle(string url)
        {
            char[] data = new char[299];
            System.Net.HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.AddRange("bytes", 0, 299);
            HttpWebResponse wre = (HttpWebResponse)wr.GetResponse();
            StreamReader sr = new StreamReader(wre.GetResponseStream());
            sr.Read(data, 0, 299);
            Console.WriteLine((data));
            sr.Close();

            return new string(data);
        }

        public static string GetWebPageTitle(string url)
        {
            // Create a request to the url
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;

            // If the request wasn’t an HTTP request (like a file), ignore it
            if (request == null) return null;

            // Use the user’s credentials
            request.UseDefaultCredentials = true;

            // Obtain a response from the server, if there was an error, return nothing
            HttpWebResponse response = null;
            try { response = request.GetResponse() as HttpWebResponse; }
            catch (WebException) { return null; }

            // Regular expression for an HTML title
            string regex = @"(?<=<title.*>)([\s\S]*)(?=</title>)";

            // If the correct HTML header exists for HTML text, continue
            if (new List<string>(response.Headers.AllKeys).Contains("Content-Type"))
                if (response.Headers["Content-Type"].StartsWith("text/html"))
                {
                    // Download the page
                    WebClient web = new WebClient();
                    web.UseDefaultCredentials = true;
                    string page = web.DownloadString(url);


                    // Extract the title
                    Regex ex = new Regex(regex, RegexOptions.IgnoreCase);
                    return ex.Match(page).Value.Trim();
                }

            // Not a valid HTML page
            return null;
        }
        protected void TextBox1_TextChanged(object sender, EventArgs e)
        { 

            if ( TextBox1.Text.Length >0 &&  TextBox1.Text[TextBox1.Text.Length-1]  == ' ')
            {
                var splited = TextBox1.Text.Split( new char[] {' ' , '\n'});
                string curr = splited[splited.Length - 1]; 
                if ( curr== "")
                    curr = splited[splited.Length - 2];
                if (CheckBox1.Checked) // spell checking 
                {
                    Dictionary<string, List<string>> words = new Dictionary<string, List<string>>();
                    words["aboard"] = get_bigrams("aboard");
                    words["lord"] = get_bigrams("lord");
                    words["border"] = get_bigrams("border");
                    words["ardent"] = get_bigrams("ardent");
                    words["any"] = get_bigrams("any");
                    words["rdrdrd"] = get_bigrams("rdrdrdr");
                    words["blabla"] = get_bigrams("blabla");
                    words["tamatmya"] = get_bigrams("tamatmya");
                    words["tamatmya"] = get_bigrams("otaya");
                    words["rozya"] = get_bigrams("rozya");
                    var words1 = related_words(curr, words);

                    words1.Sort((x, y) => CalcLevenshteinDistance(x, curr));
                    ListBox1.Items.Clear();
                    foreach (var item in words1)
                    {
                        ListBox1.Items.Add(item);
                    }
                }

                    //soundex
                if ( CheckBox2.Checked &&  curr != "")
                {
                    string Scode = Soundex(curr);
                    List<string> wordsHaveSameSound = getListOfWordsFromDB(Scode);
                    ListBox1.Items.Clear();
                    foreach (var item in wordsHaveSameSound)
                    {
                        ListBox1.Items.Add(item);
                    }
                }
                
                curr = curr; 
            }
        }

        private List<string> getListOfWordsFromDB(string Scode)
        {

           
            connection.Open();
            string s = "";
            MySqlCommand searchCmd = new MySqlCommand();
            searchCmd.CommandText = "select words from soundexIndex where code = '" + Scode + "'";
            searchCmd.Connection = connection;
            var read = searchCmd.ExecuteReader();
            //int orange=0;
         if (    read.Read()) 
                s = read["words"].ToString();
          
            connection.Close();

            return s.Split(',').ToList(); 
        }

        private string Soundex(string query)
        {
            query = query.ToLower();
            StringBuilder final_string = new StringBuilder();
            if (query != null && query.Length > 0)
            {
                string prev_code = "", current_code = "", current_letter = "";
                final_string.Append(query.Substring(0, 1));

                for (int i = 1; i < query.Length; i++)
                {
                    current_letter = query.Substring(i, 1);
                    current_code = "";
                    if ("aeiouhwy".IndexOf(current_letter) > -1)
                        current_code = "0";
                    else if ("bfvp".IndexOf(current_letter) > -1)
                        current_code = "1";
                    else if ("cjgkqsxz".IndexOf(current_letter) > -1)
                        current_code = "2";
                    else if ("dt".IndexOf(current_letter) > -1)
                        current_code = "3";
                    else if ("l".IndexOf(current_letter) > -1)
                        current_code = "4";
                    else if ("mn".IndexOf(current_letter) > -1)
                        current_code = "5";
                    else if (current_letter == "r")
                        current_code = "6";

                    if (current_code != prev_code)
                    {
                        if (current_code != "0")
                            final_string.Append(current_code);
                    }
                    prev_code = current_code;

                    if (final_string.Length == 4)
                        break;
                }
            }
            if (final_string.Length < 4)
                final_string.Append(new string('0', 4 - final_string.Length));

            return final_string.ToString().ToUpper();
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }



        Dictionary<int, int> parse_freq(string s)
        {
            Dictionary<int ,int > dic = new Dictionary<int,int>();
            if (string.IsNullOrEmpty(s))
                return null ;

            var splited = s.Split('/');
            foreach (var item in splited)
            {
                var splited2 = item.Split(':');
                if (string.IsNullOrEmpty(splited2[0]) ||string.IsNullOrEmpty(splited2[1]))
                    continue;
                int docId , freq ;
                int.TryParse(splited2[0], out docId);
                int.TryParse(splited2[1], out freq);

                dic.Add(docId,freq);
            }
            return dic; 
        }

        protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ListBox1_SelectedIndexChanged1(object sender, EventArgs e)
        {
            
        }

        protected void CheckBox1_CheckedChanged1(object sender, EventArgs e)
        {

        }
        

    }
}