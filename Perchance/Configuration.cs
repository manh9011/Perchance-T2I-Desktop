using System.Xml.Serialization;

namespace Perchance
{
    [Serializable]
    public class Configuration
    {
        [XmlIgnore]
        public string Description { get; set; } = "";
        [XmlIgnore]
        public string Negative { get; set; } = "";
        [XmlIgnore]
        public DateTime CreatedDate { get; set; }

        public string Language { get; set; } = "en";
        public string RawDescription { get; set; } = "attractive woman";
        public string RawNegative { get; set; } = "";
        public string ArtStyle { get; set; } = "Painted Anime";
        public int GuidanceScale { get; set; } = 7;
        public int Width { get; set; } = 512;
        public int Height { get; set; } = 768;
        public int Count { get; set; } = 3;
        public string? PokemonType { get; set; }

        public static string MD5(string input)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }

        [XmlIgnore]
        public string Hash => MD5(ArtStyle + "###" + Language + "###" + RawDescription + "###" + RawNegative);

        public string ToXml()
        {
            var xs = new XmlSerializer(typeof(Configuration));
            using var tw = new StringWriter();
            xs.Serialize(tw, this);
            return tw.ToString();
        }

        public static Configuration Load(string? directory = null)
        {
            try
            {
                var dir = Path.Combine(directory ?? AppDomain.CurrentDomain.BaseDirectory, "config.xml");
                var xs = new XmlSerializer(typeof(Configuration));
                using var tw = new StreamReader(dir);
                var c = (Configuration)xs.Deserialize(tw)!;
                c.CreatedDate = new FileInfo(dir).CreationTime;
                return c;
            }
            catch
            {
                return new Configuration() { CreatedDate = DateTime.MinValue };
            }
        }

        public void CopyFrom(Configuration cfg)
        {
            RawDescription = cfg.RawDescription;
            RawNegative = cfg.RawNegative;
            Language = cfg.Language;
            ArtStyle = cfg.ArtStyle;
            Width = cfg.Width;
            Height = cfg.Height;
            Count = cfg.Count;
            PokemonType = cfg.PokemonType;
        }

        public void Save()
        {
            var xml = ToXml();

            // Write last config
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
            File.WriteAllText(dir, xml);

            // Write history config
            dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history", Hash);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "config.xml"), xml);
        }
    }
}
