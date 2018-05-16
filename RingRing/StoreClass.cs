using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RingRing
{
    public class Store
    {
        private static int _Id = -1;
        private string _TvsId;
        public Store(string TvsId, string StoreName)
        {
            _Id++;
            this._TvsId = TvsId;
            this.StoreName = StoreName;
        }

        public int Id { get { return _Id; } }
        public string TvsId
        {
            get
            {
                string value = "";
                for (int i = 0; i < this._TvsId.Length; i++)
                {
                    value += this._TvsId[i];
                    if (i % 3 == 2)
                    {
                        value += " ";
                    }
                }
                return value.Trim();
            }
        }

        public string fullTvsId
        {
            get
            {
                return "TVS ID: " + TvsId;
            }
        }
        public string StoreName { get; set; }
        public override string ToString()
        {
            return this.TvsId + " , " + this.StoreName;
        }
    }
}
