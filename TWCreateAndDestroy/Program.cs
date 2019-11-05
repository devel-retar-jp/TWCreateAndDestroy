////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>
///
///     Follow / Unfollow Tool
///
///         製造 : Retar.jp   
///         Ver 1.00  2019/10/18    Destroyのみ
///         Ver 1.01  2019/11/05    Createを追加
///                                 ソース整える
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using CoreTweet;                                                //追加してください。（Nugetからパッケージを取得）
using System.Net;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Json;                        //参照で追加
using System.Windows.Forms;                                     //参照で追加

namespace TWCreateAndDestroy
{
    /// <summary>
    ///設定 #Define 
    ///     同一実行Dirにsg.jsonを入れましょう
    /// </summary>
    static class Constants
    {
        public const string sgFileNameDefault = "sg.json";      //設定ファイル
    }

    /// <summary>
    ///設定の定義 class
    ///     同一実行Dirにsg.jsonを入れましょう
    /// </summary>
    public class SG_JSON
    {
        /// <summary>
        ///Consumer API keys (API key)
        /// </summary>
        public string ConsumerKey { get; set; }
        /// <summary>
        ///Consumer API keys (API secret key)
        /// </summary>
        public string ConsumerSecret { get; set; }
        /// <summary>
        ///Access token & access token secret  (Access token)
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        ///Access token & access token secret  (Access token secret)
        /// </summary>
        public string AccessSecret { get; set; }
        /// <summary>
        ///取得インターバル 秒単位*1000
        /// </summary>
        public int sleeptime { get; set; }
        /// <summary>
        ///The screen name of the user for whom to return results.
        /// </summary>
        public string parm_screen_name { get; set; }
        /// <summary>
        //GetDeleteUserListFile ユーザーリストファイル
        /// </summary>
        public string GetUserListFile { get; set; }
        /// <summary>
        //GetFriendshiptype フォローするか、きるか Create / Destroy
        /// </summary>
        public string GetFriendshiptype { get; set; }
        /// <summary>
        ///GetScreennameOrUID  user_id / screen_name
        /// </summary>
        public string GetScreennameOrUID { get; set; }
    }

    /// <summary>
    /// Main
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            /// <summary>
            ///設定ファイル
            /// </summary>
            string fileName = Constants.sgFileNameDefault;

            /// <summary>
            ///起動引数から設定ファイル名を取得
            /// </summary>
            if (args.Length > 0) { fileName = args[0]; }

            /// <summary>
            ///ファイルの存在チェック
            /// </summary>
            SG_JSON sgjson = new SG_JSON();
            if (System.IO.File.Exists(fileName))
            {
                /// <summary>
                ///シリアライザ
                /// </summary>
                DataContractJsonSerializer sgjs = new DataContractJsonSerializer(typeof(SG_JSON));
                /// <summary>
                ///ファイルストリーム・オープン
                /// </summary>
                FileStream sgfs = new FileStream(fileName, FileMode.Open);
                /// <summary>
                ///JSONオブジェクトに設定
                /// </summary>
                sgjson = (SG_JSON)sgjs.ReadObject(sgfs);
                /// <summary>
                ///ファイルストリーム・クローズ
                /// </summary>
                sgfs.Close();
            }
            else
            {
                /// <summary>
                ////異常終了
                /// </summary>
                MessageBox.Show("'" + fileName + "'がありません。終了");
                Environment.Exit(0);    //異常終了
            }

            /// <summary>
            ///設定読み込み
            /// </summary>          
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            /// <summary>
            ///Consumer API keys (API key)
            /// </summary>          
            string ConsumerKey = sgjson.ConsumerKey;
            Console.WriteLine("sgjson.ConsumerKey : {0}", sgjson.ConsumerKey);
            /// <summary>
            ///Consumer API keys (API secret key)
            /// </summary>          
            string ConsumerSecret = sgjson.ConsumerSecret;
            Console.WriteLine("sgjson.ConsumerSecret : {0}", sgjson.ConsumerSecret);
            /// <summary>
            ///Access token & access token secret  (Access token)
            /// </summary>          
            string AccessToken = sgjson.AccessToken;
            Console.WriteLine("sgjson.AccessToken : {0}", sgjson.AccessToken);
            /// <summary>
            ///Access token & access token secret  (Access token secret)
            /// </summary>          
            string AccessSecret = sgjson.AccessSecret;
            Console.WriteLine("sgjson.AccessSecret : {0}", sgjson.AccessSecret);
            /// <summary>
            ///取得インターバル 秒単位*1000
            /// </summary>          
            int sleeptime = sgjson.sleeptime;
            Console.WriteLine("sgjson.sleeptime : {0}", sgjson.sleeptime);
            /// <summary>
            ///The screen name of the user for whom to return results.
            /// </summary>          
            string parm_screen_name = sgjson.parm_screen_name;
            Console.WriteLine("sgjson.parm_screen_name : {0}", sgjson.parm_screen_name);
            /// <summary>
            ///GetDeleteUserListFile ユーザーリストファイル
            /// </summary>          
            string getUserListFile = sgjson.GetUserListFile;
            /// <summary>
            ///GetFriendshiptype フォローするか、きるか Create / Destroy
            /// </summary>          
            string getFriendshiptype = sgjson.GetFriendshiptype;
            /// <summary>
            ///GetScreennameOrUID  user_id / screen_name
            /// </summary>          
            string getScreennameOrUID = sgjson.GetScreennameOrUID;
            
            /// <summary>
            ///Twitter API接続
            /// </summary>          
            try
            {
                /// <summary>
                ///ユーザプロファイルの取得
                /// </summary>          
                var uidparm = new Dictionary<string, object>();            //条件指定用Dictionary
                uidparm["screen_name"] = parm_screen_name;
                Tokens uidtokens = Tokens.Create(ConsumerKey, ConsumerSecret, AccessToken, AccessSecret);
                var uidtl = uidtokens.Statuses.UserTimeline();
                foreach (var item in uidtl)
                {
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine($"User.Id: {item.User.Id}");
                    Console.WriteLine($"ScreenName: {item.User.ScreenName}");
                    Console.WriteLine($"Name: {item.User.Name}");
                    Console.WriteLine("-----------------------------------------------------------------");
                    break;
                }

                /// <summary>
                ///ファイルからフォロー外しリストを読み込む
                /// </summary>          
                List<string> Ids = new List<string>();                     //フォローリストの読み込み
                try
                {
                    using (StreamReader sr = new StreamReader(getUserListFile, Encoding.Unicode))
                    {
                        while (sr.EndOfStream == false)
                        {
                            var line = sr.ReadLine();
                            Ids.Add(line);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("GetUserListFile : " + getUserListFile + "の設定異常。終了");
                    Environment.Exit(0);                    //プログラム終了
                }

                /// <summary>
                /// Destroy / Create
                /// </summary>          
                int lineCounter = 1;
                Tokens tokens = Tokens.Create(ConsumerKey, ConsumerSecret, AccessToken, AccessSecret);
                foreach (var Id in Ids)
                {
                    try
                    {
                        /// <summary>
                        ///パラメータ
                        /// </summary>          
                        var parm = new Dictionary<string, object>();            //条件指定用Dictionary

                        /// <summary>
                        ///Screenname Or UID
                        /// </summary>          
                        switch (getScreennameOrUID)
                        {
                            case "user_id":
                                parm["user_id"] = Id.ToString();
                                break;
                            case "screen_name":
                                parm["screen_name"] = Id.ToString();
                                break;
                            default:
                                MessageBox.Show("GetScreennameOrUID : " + getScreennameOrUID + "の設定異常。終了");
                                Environment.Exit(0);                    //プログラム終了
                                break;
                        }

                        /// <summary>
                        /// Twitterからのレスポンス
                        /// </summary> 
                        UserResponse DResponse = new UserResponse();
                        switch (getFriendshiptype)
                        {
                            case "Destroy":
                                //https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/post-friendships-destroy
                                DResponse = tokens.Friendships.Destroy(parm);
                                break;
                            case "Create":
                                //https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/post-friendships-create
                                DResponse = tokens.Friendships.Create(parm);
                                break;
                            default:
                                MessageBox.Show("GetFriendshiptype : " + getFriendshiptype + "の設定異常。終了");
                                Environment.Exit(0);                    //プログラム終了
                                break;
                        }

                        /// <summary>
                        /// 待ち時間
                        /// </summary> 
                        Thread.Sleep(sleeptime);

                        /// <summary>
                        /// コンソール出力
                        /// </summary> 
                        //Console.WriteLine($"{String.Format("{0:000000}", lineCounter)} : Friendships UserID : {Id.ToString()} : IsFollowRequestSent : {DResponse.IsFollowRequestSent}");
                        Console.WriteLine($"{String.Format("{0:000000}", lineCounter)} : Friendships UserID : {Id.ToString()}");
                    }
                    catch
                    {
                        /// <summary>
                        /// コンソール失敗出力
                        /// </summary> 
                        Console.WriteLine($"{String.Format("{0:000000}", lineCounter)} : ****** Fail ****** : {Id.ToString()}");
                    }
                    lineCounter++;
                }
            }
            catch (TwitterException e)
            {
                /// <summary>
                /// CoreTweetエラー
                /// </summary>
                Console.WriteLine("****** CoreTweet Error : {0}", e.Message);
                Console.ReadKey();
            }
            catch (System.Net.WebException e)
            {
                /// <summary>
                ///インターネット接続エラー
                /// </summary>
                Console.WriteLine("****** Internet Error : {0}", e.Message);
                Console.ReadKey();
            }
            /// <summary>
            ///終了
            /// </summary>
            Console.WriteLine("処理終了 : キー入力");
            Console.ReadKey();
        }
    }
}
