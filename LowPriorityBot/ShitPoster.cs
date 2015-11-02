using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LowPriorityBot
{
    class ShitPoster
    {
        Reddit reddit;
        AuthenticatedUser user;
        Subreddit subreddit;
        Queue<Post> queue;
        public string shitpostMessage = "";
        ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        Thread postFetcher;
        Thread shitposter;
        public volatile bool isJobDone = false;

        public ShitPoster()
        {
            reddit = new Reddit(false);
            queue = new Queue<Post>();
        }

        public void login(string username, string password)
        {
            user = reddit.LogIn(username, password);
            subreddit = reddit.GetSubreddit("/r/Dota2");
        }

        public void startShitposting()
        {
            postFetcher = new Thread(getLowPriorityPost);
            postFetcher.Start();
            shitposter = new Thread(doTheShitPost);
            shitposter.Start();
            postFetcher.Join();
            shitposter.Join();
        }

        public void getLowPriorityPost()
        {
            while (!isJobDone)
            {
                bool isSkipSleep = false;
                Console.WriteLine("[{0}] checking...", DateTime.Now.ToShortTimeString());
                if (_shutdownEvent.WaitOne(0))
                {
                    break;
                }
                try
                {
                    subreddit = reddit.GetSubreddit("/r/Dota2");
                }
                catch (WebException ex)
                {
                    isSkipSleep = true;
                    Console.WriteLine("[{0}] WebException... Retrying...", DateTime.Now.ToShortTimeString());
                    Thread.Sleep(10000);
                }

                try
                {
                    lock (queue)
                    {
                        IEnumerable<Post> posts = subreddit.New.Take(200);
                        foreach (Post post in posts)
                        {
                            if ((post.Title.Contains("LP ")) || (post.Title.Contains(" LP")) || (post.Title.Contains("(LP)")) || (post.Title.Contains("[LP]")) ||
                                (post.Title.Contains("LPQ ")) || (post.Title.Contains(" LPQ")) || (post.Title.Contains("(LPQ)")) || (post.Title.Contains("[LPQ]")) ||
                                (post.Title.Contains("low prio")) || (post.Title.Contains("Low priority")) || (post.Title.Contains("Low Priority")) || (post.Title.Contains("low priority")) || (post.Title.IndexOf("low priority", StringComparison.OrdinalIgnoreCase) >= 0) || (post.Title.IndexOf("low prio", StringComparison.OrdinalIgnoreCase) >= 0))
                            {
                                if (!ShitpostDBConnector.isExists(post.Permalink.ToString()))
                                {
                                    if (!queue.Contains(post))
                                    {
                                        queue.Enqueue(post);
                                        Console.WriteLine(post.Permalink.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                catch (WebException wex)
                {
                    Console.WriteLine(wex);
                    Console.WriteLine("[{0}] retrying...", DateTime.Now.ToShortTimeString());
                    isSkipSleep = true;
                    Thread.Sleep(10000);
                }
                
                if (!isSkipSleep)
                {
                    Thread.Sleep(300000);
                }
            }
        }

        public void doTheShitPost()
        {
            while (!isJobDone)
            {
                if (_shutdownEvent.WaitOne(0))
                {
                    break;
                }
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        Post lpPost = queue.Dequeue();
                        if (!ShitpostDBConnector.isExists(lpPost.Permalink.ToString()))
                        {
                            lpPost.Comment(shitpostMessage);
                            ShitpostDBConnector.addLink(lpPost.Permalink.ToString());
                            Console.WriteLine("[{0}] comment sent", DateTime.Now.ToShortTimeString());
                            lpPost = null;
                            Thread.Sleep(600000);
                        }
                        Thread.Sleep(10000);
                    }
                }
                Thread.Sleep(10000);
            }
        }

        public void stop()
        {
            _shutdownEvent.Set();
        }
    }
}
