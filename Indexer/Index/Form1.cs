using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Index
{
    public partial class Form1 : Form
    {
        PorterStemming ps = new PorterStemming();
        List<TermClass> ListOfTerms = new List<TermClass>();
        List<string> contentOfAllPages = new List<string>();
        List<string> filtercontent = null;



        public Form1()
        {
            InitializeComponent();
          //  insertWordBFStem(new HashSet<string>() { "scddas", "fdgpoe" }); 

        }


        

        struct pair
        {
            public int i, j;
            public pair( int _i , int _j )
            {
                i = _i;
                j = _j; 
            }
        }
        struct doc
        {
           public String content;
          public  int docID; 
        }

        void doIndexing()
        {

            GetContent();
            for (int i = 0; i < contentOfAllPages.Count; i++)
            {
                Dictionary<string, List<int>> postion_dictionaery = new Dictionary<string,List<int>>();
                Dictionary<string, int> frequncy_dictionaery= new Dictionary<string,int>();
                List<string> filtercontent = Parse(contentOfAllPages[i], ref  postion_dictionaery, ref  frequncy_dictionaery);
                for (int j = 0; j < filtercontent.Count; j++)
                {
                    TermClass mystr = new TermClass();
                    bool flag = false;
                    var item111 = ListOfTerms.Find((x) => x.term == filtercontent[j]);

                    TermClass s1 = new TermClass();
                    if (item111 == null)
                    {
                       
                        s1.term = filtercontent[j];
                        s1.frequncy = 0;
                        s1.postion = new Dictionary<int, List<int>>();
                        s1.doc = new List<int>();             
                        ListOfTerms.Add(s1);
                    }
                    else
                    {
                        s1 = item111; 
                    }

                    s1.frequncy += frequncy_dictionaery[filtercontent[j]];
                   
                        s1.postion[i] = postion_dictionaery[filtercontent[j]];
                    s1.doc.Add(i);
               
                }
                //    progressBar1.Value = (int)(((float)i / (float)contentOfAllPages.Count) * 100);
                //   progressBar1.Update(); 
                //  await Task.Yield();
            }
            //   MessageBox.Show("finished Indexing now Inserting"); 
            for (int a = 0; a < ListOfTerms.Count; a++)
            {
                //int[] m = new int[1000000];
                List<int> m = new List<int>();
                string term = ListOfTerms[a].term;
                string docid = "";
                string Positions = "";
                int frequncy = ListOfTerms[a].frequncy;
                for (int v = 0; v < ListOfTerms[a].doc.Count; v++)
                {
                    docid += ListOfTerms[a].doc[v];
                    docid += ",";
                    m.Add(ListOfTerms[a].doc[v]);
                }
                docid = docid.Remove(docid.Length - 1);

                for (int v = 0; v < ListOfTerms[a].postion.Count; v++)
                {
                    int d;
                    d = ListOfTerms[a].postion[m[v]].Count;   
                    for (int k = 0; k < d; k++)
                    {
                        Positions += m[v];
                        Positions += ":";
                        Positions += ListOfTerms[a].postion[m[v]][k];
                        Positions += ",";
                    }
                    Positions = Positions.Remove(Positions.Length - 1);
                    Positions += "/";
                }
                // docid = docid.Remove(docid.Length - 1);
                Positions = Positions.Remove(Positions.Length - 1);
                //Insert(term, docid, frequncy, Positions);

                if (a % 20 == 0)
                {
                    progressBar1.Value = (int)(((float)a / (float)ListOfTerms.Count) * 100);
                    progressBar1.Update();
                    //      await Task.Yield();
                }
            }
            MessageBox.Show("finished Inserting");
        }


       void    DeuxIndexing(List<doc> docs )
        {
            MessageBox.Show("begin indexing");
            string Doc_One = "The bright blue butterfly hangs on the breeze";
            string Doc_Two = "forget It is best to forget the great hangs sky and retire from every wind bright شسيشسي";
            Dictionary<string, List<pair>> indexed = new Dictionary<string, List<pair>>();

            HashSet<string> stopWords = new HashSet<string>();
            stopWords.Add("a");
            stopWords.Add("and");
            stopWords.Add("around");
            stopWords.Add("every");
            stopWords.Add("for");
            stopWords.Add("from");
            stopWords.Add("in");
            stopWords.Add("is");
            stopWords.Add("it");
            stopWords.Add("not");
            stopWords.Add("on");
            stopWords.Add("one");
            stopWords.Add("the");
            stopWords.Add("The");
            stopWords.Add("two");
            stopWords.Add("under");

            //List<doc> docs = new List<doc>(3);
            //docs.Add(new doc() { content = Doc_One, docID= 5 } );
            //docs.Add(new doc() { docID = 3, content = Doc_Two });


            for (int i = 0; i < docs.Count; i++)
            {

                string[] splitted = docs[i].content.Split(new char[] { ',', '\r', '\n', ' ', '=', '\\', '/', '<', '>', '"', '!', '-', ':', ';' });
                for (int j = 0; j < splitted.Length; j++)
                {

                    var item =  splitted[j].ToLower();

                    if (!(Regex.IsMatch(item, @"^[a-zA-Z|]+$"))) //english 
                        continue;

                    if ( !WordsBeforeStemming.Contains(item))
                    WordsBeforeStemming.Add(item);

                  item = ps.stem(item); 
                    if (!stopWords.Contains(item) )
                    {
                        if (!indexed.ContainsKey(item))
                        {

                            indexed.Add(item, new List<pair>());
                        }
                        var docID = docs[i].docID; 
                        indexed[item].Add(new pair(docID, j));
                    }

                }
                progressBar1.Value = (int)(((float)i / (float)docs.Count) * 100);
                progressBar1.Update();
                Console.WriteLine("working on " + i +" of " + docs.Count);
               // await Task.Yield();

            }
            int Progress = 0;
            int All = indexed.Keys.Count;
            foreach (var item in indexed)
            {
                Progress++;
                Console.WriteLine(item.Key);


                string Term = item.Key;
                string Pos = "" , Freq="" , PostionalIndex="" ;
                Dictionary<int, List<int>> docID_pos = new Dictionary<int, List<int>>();
                for (int i = 0; i < item.Value.Count; i++)
                {
                  //  Console.Write(" in doc id : " + item.Value[i].i + " " + "pos: " + item.Value[i].j);
                    var dID = item.Value[i].i;
                    var PosInDid = item.Value[i].j;
                    if (!docID_pos.ContainsKey(dID))
                    {
                        docID_pos.Add(dID, new List<int>());
                    }
                    docID_pos[dID].Add(PosInDid);

                }

                foreach (var docID in docID_pos.Keys)
                {
                    Pos += docID;
                    Pos += ",";
                }

                foreach (var postions in docID_pos)
                {
                    Freq += postions.Key + ":" + postions.Value.Count;
                    Freq += "/"; 

                }

                foreach (var positions2 in docID_pos)
                {
                    for (int i = 0; i < positions2.Value.Count; i++)
                    {
                        PostionalIndex += positions2.Key +":" + positions2.Value[i] +",";
                    }

                    PostionalIndex += "/";
                }

                Console.WriteLine();

                try
                {
 Insert(Term, Pos, Freq, PostionalIndex);
                }
                catch (Exception)
                {
                    
                    throw;
                }
               

                progressBar1.Value = (int)(((float)Progress / (float)All) * 100);
                progressBar1.Update();
              //  await Task.Yield();
               
                //Console.WriteLine(item.Key);
            }

            MessageBox.Show("indexing and inserting Done ");
            Console.WriteLine(WordsBeforeStemming.Count); 
            insertWordBFStem(WordsBeforeStemming); 
         
        }



        private void insertWordBFStem(HashSet<string> WordsBeforeStemming)
        {
            con.Open();
            MySqlCommand searchCmd = new MySqlCommand();
            string cmdText = " insert into wordsBeforeStemming (word) values ";

            searchCmd.Connection = con;
            foreach (var item in WordsBeforeStemming)
            {
                cmdText += string.Format("('{0}'),", item);
            }
         cmdText=   cmdText.Remove(cmdText.Length - 1);
            searchCmd.CommandText = cmdText;
            searchCmd.ExecuteNonQuery();
            con.Close(); 
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static string conectionSTR = "server=localhost;user=root;database=crawler;port=3306;";


        private void GetContent()
        {

            MySqlConnection con = new MySqlConnection(conectionSTR);
            con.Open();
            MySqlCommand searchCmd = new MySqlCommand();
            searchCmd.CommandText = "SELECT *   FROM mylinks where ID<1500";
            searchCmd.Connection = con;
            var read = searchCmd.ExecuteReader();
            while (read.Read())
            {
                contentOfAllPages.Add(read["content"].ToString());
            }
            con.Close();
        }


        private  List<doc> GetDocs()
        {
            List<doc> docs = new List<doc>(); 
            MySqlConnection con = new MySqlConnection(conectionSTR);
            con.Open();
            MySqlCommand searchCmd = new MySqlCommand();
            searchCmd.CommandText = "SELECT *   FROM mylinks2 where ID<1500";
            searchCmd.Connection = con;
            var read = searchCmd.ExecuteReader();
            while (read.Read())
            {
                     docs.Add( new doc() { content =  read["content"].ToString() , docID = int.Parse(read["id"].ToString()) }) ;
            }
            con.Close();


            return docs; 
        }

        //to be added to Kgramm index and Soundex 
        HashSet<string> WordsBeforeStemming = new HashSet<string>();
        private List<string> Parse(string s, ref  Dictionary<string, List<int>> postion_dictionaery, ref  Dictionary<string, int> frequncy_dictionaery)
        {
            //   s = s.Substring(s.IndexOf("body"));
            List<string> eng = new List<string>();
            char[] delimiters = new[] { ',', '\r', '\n', ' ', '=', '\\', '/', '<', '>', '"', '!', '-', ':', ';' };
            string[] stopwords_punctuation =
                new[] { "a ", "and", "from", "for", "in", "is", "it", "not", "on", "the", ".", "?!", "?", "!", "id", "div", "li", "to", "class", "name", "value", "type", "showgrid", "width", "hidden", "input", "style", "href", "a", "img", "src", "title", "nav", "ul", "http", "mritems" };
            List<string> spliter = s.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
            spliter = spliter.ConvertAll(d => d.ToLower());
            for (int n = 0; n < spliter.Count; n++)
            {
                if (Regex.IsMatch(spliter[n], @"^[a-zA-Z|]+$"))
                {
                    if (!WordsBeforeStemming.Contains(spliter[n]))
                        WordsBeforeStemming.Add(spliter[n]);

                }
                spliter[n] = ps.stem(spliter[n]);
            }

            for (int a = 0; a < spliter.Count; a++)
            {
                if (spliter[a].Length < 3)
                {
                    spliter.Remove(spliter[a]);
                }
            }
            for (int w = 0; w < spliter.Count; w++)
            {
                //english
                if (Regex.IsMatch(spliter[w], @"^[a-zA-Z|]+$"))
                {

                    if (!eng.Contains(spliter[w]))    //lwo mesh mwgod fy eng list haroo7 adefo
                    {
                        eng.Add(spliter[w]);
                        List<int> postion = new List<int>();

                        postion.Add(w + 1);
                        try
                        {
                            postion_dictionaery.Add(spliter[w], postion);
                            frequncy_dictionaery.Add(spliter[w], 1);
                        }
                        catch (Exception e)
                        {

                        }
                    }


                    else
                    {
                        postion_dictionaery[spliter[w]].Add(w + 1);
                        frequncy_dictionaery[spliter[w]] += 1;
                    }
                }

            }


            for (int a = 0; a < eng.Count; a++)
            {

                for (int b = 0; b < stopwords_punctuation.Length; b++)
                {
                    if (eng[a].ToLower() == stopwords_punctuation[b].ToLower())
                    {
                        //  eng[a].Replace(eng[a], "");
                        string str = eng[a];
                        eng.Remove(str);
                        frequncy_dictionaery.Remove(str);
                        postion_dictionaery.Remove(str);
                        if (a != 0)
                            a--;
                    }
                }
            }
            return eng;
        }
        MySqlConnection con = new MySqlConnection(conectionSTR);
        private void Insert(string term, string docID, string freq, string pos)
        {
            //return;
            
            con.Open();
            MySqlCommand searchCmd = new MySqlCommand();
            searchCmd.CommandText = "insert into indexedTerms (Term , DocID , Frequency , Position) VALUES(@n,@p,@e,@w)";
            searchCmd.Parameters.AddWithValue("@n", term);
            searchCmd.Parameters.AddWithValue("@p", docID);
            searchCmd.Parameters.AddWithValue("@e", freq);
            searchCmd.Parameters.AddWithValue("@w", pos);
            searchCmd.Connection = con;
            searchCmd.ExecuteScalar();
            con.Close();
        }
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


        async Task InsertKgramgs()
        {
            Dictionary<string, List<string>> kGramIndex = new Dictionary<string, List<string>>();
            foreach (var item in WordsBeforeStemming)
            {
                var biS = get_bigrams(item);

                foreach (var bi in biS)
                {
                    if (!kGramIndex.ContainsKey(bi))
                    {
                        kGramIndex.Add(bi, new List<string>());

                    }

                    if (!kGramIndex[bi].Contains(item))
                        kGramIndex[bi].Add(item);
                }
            }

            MessageBox.Show("kgrammIndexingFinished");
            MySqlConnection con = new MySqlConnection(conectionSTR);
            int i = 0;
            foreach (var item in kGramIndex)
            {
                con.Open();
                MySqlCommand searchCmd = new MySqlCommand();
                searchCmd.CommandText = "insert into KgramIndex (biGram , Words) VALUES(@n,@p)";
                searchCmd.Parameters.AddWithValue("@n", item.Key);

                string listOfWordOfBi = "";
                foreach (var worddd in item.Value)
                {
                    listOfWordOfBi += worddd + ",";
                }
                searchCmd.Parameters.AddWithValue("@p", listOfWordOfBi);
                searchCmd.Connection = con;
                searchCmd.ExecuteScalar();
                con.Close();

                progressBar1.Value = (int)(((float)i / (float)kGramIndex.Keys.Count) * 100);
                progressBar1.Update();
                i++;
                await Task.Yield();
            }
            MessageBox.Show("kgrammIndexingFinished and inserted to DB");
        }


        public void testKgramIndex()
        {
            //Please stay on topic, stay friendly, use english and don´t spam/advertise - 
            //if you need basic tutorials/infos/code samples, please use google or the 


            WordsBeforeStemming.Add("Please");
            WordsBeforeStemming.Add("stay");
            WordsBeforeStemming.Add("on");
            WordsBeforeStemming.Add("stay");
            WordsBeforeStemming.Add("friendly");
            WordsBeforeStemming.Add("use");
            WordsBeforeStemming.Add("english");
            WordsBeforeStemming.Add("and");
            WordsBeforeStemming.Add("spam");
            WordsBeforeStemming.Add("advertise");

            Task.Run(() => InsertKgramgs());

        }

        public void testSoundexIndex()
        {
            //Please stay on topic, stay friendly, use english and don´t spam/advertise - 
            //if you need basic tutorials/infos/code samples, please use google or the 


            WordsBeforeStemming.Add("Please");
            WordsBeforeStemming.Add("stay");
            WordsBeforeStemming.Add("on");
            WordsBeforeStemming.Add("stay");
            WordsBeforeStemming.Add("friendly");
            WordsBeforeStemming.Add("use");
            WordsBeforeStemming.Add("english");
            WordsBeforeStemming.Add("and");
            WordsBeforeStemming.Add("spam");
            WordsBeforeStemming.Add("spom");
            WordsBeforeStemming.Add("span");
            WordsBeforeStemming.Add("spam");
            WordsBeforeStemming.Add("spawn");
            WordsBeforeStemming.Add("sphene");
            WordsBeforeStemming.Add("spinny");

            Task.Run(() => InsertSoundexIndex());

        }
        async Task InsertSoundexIndex()
        {
            Dictionary<string, List<string>> soundexIndex = new Dictionary<string, List<string>>();
            foreach (var item in WordsBeforeStemming)
            {
                var bi = Soundex(item);

                if (!soundexIndex.ContainsKey(bi))
                {
                    soundexIndex.Add(bi, new List<string>());

                }

                if (!soundexIndex[bi].Contains(item))
                    soundexIndex[bi].Add(item);

            }

            MessageBox.Show("soundexIndexingFinished");
            MySqlConnection con = new MySqlConnection(conectionSTR);
            int i = 0;
            foreach (var item in soundexIndex)
            {
                con.Open();
                MySqlCommand searchCmd = new MySqlCommand();
                searchCmd.CommandText = "insert into soundexIndex (code , Words) VALUES(@n,@p)";
                searchCmd.Parameters.AddWithValue("@n", item.Key);

                string listOfWordOfBi = "";
                foreach (var worddd in item.Value)
                {
                    listOfWordOfBi += worddd + ",";
                }
                searchCmd.Parameters.AddWithValue("@p", listOfWordOfBi);
                searchCmd.Connection = con;
                searchCmd.ExecuteScalar();
                con.Close();

                progressBar1.Value = (int)(((float)i / (float)soundexIndex.Keys.Count) * 100);
                progressBar1.Update();
                i++;
                await Task.Yield();
            }
            MessageBox.Show("soundexIndexingFinished and inserted to DB");
        }


        //soundex algorithm 
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

        //soundex testing
        void soundex_check()
        {
            string test = Soundex("Herman");
            string test1 = Soundex("Harman");
            string test2 = Soundex("Hermon");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var docs = GetDocs(); 
DeuxIndexing(docs );
          //  Task.Run(() => );
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (WordsBeforeStemming.Count < 5)
                getWordsBeforeStemming(); 
            testKgramIndex();
        }


        void getWordsBeforeStemming ()
        {
            
            MySqlConnection con = new MySqlConnection(conectionSTR);
            con.Open();
            MySqlCommand searchCmd = new MySqlCommand();
            searchCmd.CommandText = "SELECT word FROM wordsBeforeStemming ";
            searchCmd.Connection = con;
            var read = searchCmd.ExecuteReader();
            while (read.Read())
            {
                WordsBeforeStemming.Add(read["word"].ToString()); 
            }
            con.Close();

        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (WordsBeforeStemming.Count < 5)
                getWordsBeforeStemming(); 
            testSoundexIndex();
        }
    }
}
