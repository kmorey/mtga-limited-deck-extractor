using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace MTGA_Limited_Deck_Exporter
{
    static class Utils
    {
        private static Dictionary<int, Card> Cards { get; set; } = new Dictionary<int, Card>();

        public static int CardCount
        {
            get
            {
                return Cards.Count;
            }
        }

        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "MTGA_Limited_Deck_Exporter.scryfall-default-cards.json";

            Cards.Clear();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    var cards = JsonConvert.DeserializeObject<List<Card>>(result);
                    foreach (Card card in cards)
                    {
                        if (card.ArenaId > 0 && !Cards.ContainsKey(card.ArenaId))
                        {
                            Cards.Add(card.ArenaId, card);
                        }
                    }
                }
            }
        }

        public static Card FindCard(string messyId)
        {
            var id = int.Parse(messyId.Trim());
            return new Card(Cards[id]);
        }

        public static bool FileHasSealedPool(string filePath)
        {
            var doc = new HtmlDocument();
            doc.Load(filePath);

            var nodes = doc.DocumentNode.SelectNodes("//*[contains(text(), 'SealedPoolGenerated')]");
            return nodes != null;
        }

        public static List<Pool> FindSealedPools(string filePath)
        {
            var doc = new HtmlDocument();
            doc.Load(filePath);

            var pools = new Dictionary<int, Pool>();
            var nodes = doc.DocumentNode.SelectNodes("//*[contains(text(), 'SealedPoolGenerated')]");
            foreach (var node in nodes)
            {
                var messyCardList = new Regex("CardPool\": \\[([^\\]]+)").Match(node.InnerHtml).Groups[1].Value.Split(',');
                var pool = new Pool();
                foreach (var messyId in messyCardList)
                {
                    pool.Add(Utils.FindCard(messyId));
                }

                pool.UpdateDescription();

                if (!pools.ContainsKey(pool.Id))
                {
                    pools.Add(pool.Id, pool);
                }
            }

            return pools.Values.ToList();
        }

        internal static string GetExportString(object selectedItem)
        {
            var pool = (Pool)selectedItem;
            var sb = new StringBuilder();
            foreach (var card in pool)
            {
                sb.AppendFormat("{0} {1}\n", card.Count, card.Name);
            }

            return sb.ToString();
        }
    }
}
