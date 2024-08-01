using Newtonsoft.Json;

namespace ZemaPriceUpload
{
    public class ZemaPrice
    {
        [JsonProperty("priceindex")]
        public string priceindex { get; set; }

        [JsonProperty("group")]
        public string group { get; set; }

        [JsonProperty("delivdate")]
        public string delivdate { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("leveltype")]
        public string leveltype { get; set; }

        [JsonProperty("levelvalue")]
        public string levelvalue { get; set; }

        [JsonProperty("putcall")]
        public string putcall { get; set; }

        [JsonProperty("price")]
        public string price { get; set; }

        [JsonProperty("pricedate")]
        public string pricedate  { get; set; }

        [JsonProperty("callprice")]
        public string callprice { get; set; }

        [JsonProperty("putprice")]
        public string putprice { get; set; }

        [JsonProperty("strikeprice")]
        public string strikeprice { get; set; }

        [JsonProperty("contractMonth")]
        public string contractmonth { get; set; }

        [JsonProperty("contractGranularity")]
        public string contractgranularity { get; set; }

        [JsonProperty("contractYear")]
        public string contractyear { get; set; }

        [JsonProperty("contractStart")]
        public string contractstart { get; set; }

        [JsonProperty("contractEnd")]
        public string contractend { get; set; }

        [JsonProperty("uom")]
        public string uom { get; set; }

        [JsonProperty("datatype")]
        public string datatype { get; set; }

        [JsonProperty("datagroup")]
        public string datagroup { get; set; }

        [JsonProperty("productname")]
        public string productname { get; set; }

        [JsonProperty("currency")]
        public string currency { get; set; }

        [JsonProperty("productcode")]
        public string productcode { get; set; }

        [JsonProperty("indexgroup")]
        public string indexgroup { get; set; }

        [JsonProperty("source")]
        public string source { get; set; }

        [JsonProperty("oprhour")]
        public string oprhour { get; set; }

        [JsonProperty("batchnumber")]
        public string batchnumber { get; set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; set; }
    }
}
