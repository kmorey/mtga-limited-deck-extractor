using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGA_Limited_Deck_Exporter
{
    public class Pool : List<Card>
    {
        private int id = 0;
        public int Id
        {
            get
            {
                if (id == 0)
                {
                    id = BuildId();
                }
                return id;
            }

            set
            {
                id = BuildId();
            }
        }

        private int BuildId()
        {
            var sb = new StringBuilder();
            foreach (Card card in this)
            {
                sb.Append(card.Count);
                sb.Append(':');
                sb.Append(card.ArenaId);
                sb.Append(';');
            }
            return sb.ToString().GetHashCode();
        }

        private string description = null;
        public string Description
        {
            get
            {
                if (description == null)
                {
                    description = BuildDescription();
                }
                return description;
            }
            set
            {
                description = value;
            }
        }

        public string Card1 { get; set; }
        public string Card2 { get; set; }
        public string Card3 { get; set; }
        public string Card4 { get; set; }
        public string Card5 { get; set; }
        public string Card6 { get; set; }

        private string BuildDescription()
        {
            var rares = FindAll(x => x.Rarity.Equals("mythic") || x.Rarity.Equals("rare"));
            rares.Sort(comparer: new Card.RarityComparer());

            Card1 = rares[0].CardImages.Small;
            Card2 = rares[1].CardImages.Small;
            Card3 = rares[2].CardImages.Small;
            Card4 = rares[3].CardImages.Small;
            Card5 = rares[4].CardImages.Small;
            Card6 = rares[5].CardImages.Small;

            return String.Join(" // ", rares);
        }

        public new void Add(Card item)
        {
            var existing = this.Find(x => x.ArenaId == item.ArenaId);
            if (existing != null)
            {
                existing.Count += 1;
            }
            else
            {
                base.Add(item);
            }
        }

        public void UpdateDescription()
        {
            Description = BuildDescription();
        }
    }
}
