using System.Diagnostics;
using System.Text.RegularExpressions;
using static Perchance.ArtStyle;

namespace Perchance
{
    public partial class FrmMain : Form
    {
        [GeneratedRegex(@"\{([^\}]+)\}")]
        private static partial Regex ChooseRegex();

        [GeneratedRegex(@"\[([^]]+)\]")]
        private static partial Regex ImportRegex();

        private static readonly Random rand = new((int)(DateTime.Now.Ticks % int.MaxValue));
        private List<PerchanceBox> clients = [];
        private readonly Configuration cfg = Configuration.Load();

        private readonly Dictionary<string, List<string>> repl = new()
        {
            ["profession"] = ["waitress", "nurse", "librarian", "teacher", "scientist", "college professor"],
            ["animal"] = ["aardvark", "alligator", "alpaca", "antelope", "ape", "armadillo", "baboon", "badger", "bat", "bear", "beaver", "bison", "boar", "buffalo", "bull", "camel", "canary", "capybara", "cat", "chameleon", "cheetah", "chimpanzee", "chinchilla", "chipmunk", "cougar", "cow", "coyote", "crocodile", "crow", "deer", "dingo", "dog", "donkey", "dromedary", "elephant", "elk", "ewe", "ferret", "finch", "fish", "fox", "frog", "gazelle", "gila monster", "giraffe", "gnu", "goat", "gopher", "gorilla", "grizzly bear", "ground hog", "guinea pig", "hamster", "hedgehog", "hippopotamus", "hog", "horse", "hyena", "ibex", "iguana", "impala", "jackal", "jaguar", "kangaroo", "koala", "lamb", "lemur", "leopard", "lion", "lizard", "llama", "lynx", "mandrill", "marmoset", "mink", "mole", "mongoose", "monkey", "moose", "mountain goat", "mouse", "mule", "muskrat", "mustang", "mynah bird", "newt", "ocelot", "opossum", "orangutan", "oryx", "otter", "ox", "panda", "panther", "parakeet", "parrot", "pig", "platypus", "polar bear", "porcupine", "porpoise", "prairie dog", "puma", "rabbit", "raccoon", "ram", "rat", "reindeer", "reptile", "rhinoceros", "salamander", "seal", "sheep", "shrew", "silver fox", "skunk", "sloth", "snake", "squirrel", "tapir", "tiger", "toad", "turtle", "walrus", "warthog", "weasel", "whale", "wildcat", "wolf", "wolverine", "wombat", "woodchuck", "yak", "zebra", "anaconda", "anteater", "bobcat", "buzzard", "catfish", "chicken", "cobra", "crab", "cuckoo", "dinosaur", "dolphin", "dove", "duck", "eagle", "eel", "emu", "falcon", "flamingo", "goose", "hare", "hawk", "hummingbird", "lobster", "meerkat", "octopus", "ostrich", "owl", "oyster", "peacock", "pelican", "penguin", "pheasant", "pigeon", "pony", "quail", "rattlesnake", "rooster", "shark", "sparrow"],

        };

        private readonly string[] suggestion = [
            "{enigmatic|mysterious} {demon|angel|ghost} royalty {sitting on|standing beside} {their throne|a cosmic altar}, {a horde of minions|an array of celestial servants|a legion of spectral beings} {behind|in front of} then, {close-up |character |}portrait",
            "portrait of {a} [animal]-{man|woman}, {servant|god|priest|warrior|gardener|keeper} of {the underworld|the seven worlds|light|darkness|jungle world|cybercrystal|blue fire|electrical storms|the deep heart|the overworld|luxury|dragonkind}",
            "{a} {cute |^3}[animal] x [animal] hybrid, pokemon-like creature",
            "{cyborg|steampunk|magical} {mermaid|merman|dolphin} exploring {a coral reef|an underwater city|underwater dunes}",
            "{steampunk|futuristic|ancient} explorer with {an airship|a hoverbike|a time-traveling artifact}",
            "{graceful|agile|mysterious} {alien|human|robotic} dancer performing {on stage|in a street|in moonlight}",
            "{neon|sunset|moonlit} cityscape during {a rainstorm|a snowfall|a meteor shower}",
            "{magical|enchanted|cursed} forest guarded by {giant mushrooms|sentient trees|whimsical spirits}",
            "{lonely|brave|curious} lighthouse keeper with {a ghostly|an animal|a supernatural} companion",
            "{haunted|abandoned|ancient} mansion inhabited by {quirky|creepy|kind} spirits",
            "{futuristic|old-fashioned|magical} train racing through {a desert|a tundra|a jungle} landscape",
            "{underwater|sky|space} pirate captain with {a submarine|an airship|a spaceship} ship",
            "{time-traveling|interdimensional|supernatural} detective with {a pet dinosaur|a ghostly sidekick|a shape-shifting familiar}",
            "{robotic|medieval|fantasy} knight jousting on {a mechanical|a fire-breathing|a spectral} horse",
            "{ethereal|ancient|cosmic} space goddess floating among {stars|nebulae|galaxies}",
            "{wise|ancient|intelligent} {tree|rock|cloud} spirit with {a magical staff|a crystal orb|an enchanted flute}",
            "{superhero|villain|antihero} with the ability to control {plants|weather|fire}",
            "{flying|hovering|submersible} car chase through {a futuristic|an underwater|a post-apocalyptic} city",
            "{underwater|floating|hidden} cityscape lit by {bioluminescent plants|ancient crystals|magical torches}",
            "{mystical|cursed|prehistoric} cave with {glowing crystal|shimmering ice|luminescent fungus} formations",
            "{post-apocalyptic|futuristic|ancient} wanderer with {a pet robot|a loyal animal companion|a supernatural guide}",
            "{masked|armored|stealthy} vigilante perched {on a skyscraper|in a tree|atop a cliff}",
            "{sorceress|witch|alchemist} brewing {a potion|a spell|a concoction} in {her hidden lair|a secret cave|an enchanted laboratory}",
            "{steampunk|cyberpunk|fantasy} inventor with {a mechanical|a magical|a sentient} companion",
            "{cyberpunk|interstellar|paranormal} hacker in {a digital world|a secret network|an alternate reality}",
            "{enchanted|fairy-tale|secret} garden filled with {mythical|unusual|magical} creatures",
            "{cute|fierce|friendly} monster family {going on a picnic|exploring a forest|visiting a village}",
            "{astronaut|alien|time-traveler} discovering {an alien oasis|a hidden planet|a realm beyond time}",
            "{old-fashioned|modern|magical} {ice cream|candy|dessert} parlour with {magical|unusual|dangerous} flavors",
            "{abandoned|haunted|enchanted} amusement park {reclaimed by nature|inhabited by spirits|guarded by creatures}",
            "{warrior|sorceress|queen} riding {a mythical|a giant|an elemental} beast",
            "{vintage|modern|otherworldly} circus with {supernatural|magical|dangerous} performers",
            "{otherworldly|majestic|mysterious} library filled with {ancient|enchanted|forbidden} tomes",
            "{intrepid|fearless|daring} explorer navigating {a perilous|a mysterious|an enchanted} jungle",
            "{cozy|remote|enchanted} cottage in {the mountains|a forest|a magical realm} with {magical|supernatural|ancient} neighbors",
            "{street|stage|televised} magician performing {a jaw-dropping|a dangerous|an enchanting} illusion",
            "{mighty|ancient|winged} dragon {slumbering|guarding|hiding} atop {a hoard of treasure|enchanted crystals|cursed artifacts}",
            "{chibi-style|cartoon|realistic} superheroes {saving the day|battling villains|protecting a city}",
            "{whimsical|industrial|supernatural} factory where {dreams|nightmares|wishes} are manufactured",
            "{space|intergalactic|time-traveling} cowboy hunting {intergalactic outlaws|ancient threats|supernatural foes}",
            "{necromancer|summoner|witch} raising {an army of the undead|elemental creatures|magical beings}",
            "{brave|cursed|legendary} warrior with {a haunted|an enchanted|a sentient} weapon",
            "{interdimensional|cosmic|paranormal} {coffee|tea|dessert} shop frequented by {odd|magical|alien} patrons",
            "{enchanted|bewitched|sacred} forest populated by {sentient|magical|cursed} trees",
            "{cybernetic|supernatural|mystical} musician creating {otherworldly|haunting|hypnotic} tunes",
            "{alien|lost|ancient} civilization thriving on {a floating island|a hidden continent|an undiscovered planet}",
            "woman, at a {market|grocery store}, {choosing fruits|selecting produce}, sunny smile, close-up",
            "man, at a {zoo|safari}, {feeding giraffe|watching elephants}, laughing, close-up",
            "girl at a {playground|park}, on swings, {skyward|forward}, joyous, close-up",
            "{dj|musician}, {mixing|performing}, at a club, crowd, excited, close-up",
            "woman, in a {yoga pose|tai chi stance}, {peaceful garden|zen studio}, serene, close-up",
            "chef, at a {food truck|diner}, serving {tacos|burgers}, friendly wink, close-up",
            "{girl|boy}, in a {snowy landscape|rainy city}, {catching snowflakes|jumping in puddles}, amused, close-up",
            "sailor, in a {boat|dock}, {sea|lake}, {stormy weather|calm waters}, determined, close-up",
            "woman, at a {vineyard|brewery}, {tasting wine|sampling beer}, indulgent smile, close-up",
            "{man|woman}, hiking, {forest|desert|mountain}, close-up",
            "girl, at a {music store|comic shop}, {flipping through vinyl records|browsing graphic novels}, nostalgic, close-up",
            "{man|woman}, in a lab, {microscope|test tubes}, {discovery|experiment}, astonished, close-up",
            "{beautiful |}woman, at a {cafe|co-working space}, {laptop|notepad}, deep in work, focused, close-up",
            "soldier, in {uniform|camouflage}, {saluting|at attention}, proud, close-up",
            "{beautiful |}woman, in a {greenhouse|botanical garden}, {surrounded by plants|studying foliage}, peaceful, close-up",
            "man, at a {pottery wheel|loom}, {molding clay|weaving fabric}, concentrated, close-up",
            "{girl|boy}, at a {rooftop|penthouse}, {looking at stars|watching the city}, thoughtful, close-up",
            "{man|woman}, in a tattoo parlor, {getting inked|choosing designs}, excited, close-up",
            "woman, at a {basketball game|soccer match}, cheering, exuberant, close-up",
            "{tailor|dressmaker}, {fitting a suit|adjusting a dress}, {measuring tape|pins}, focused, close-up",
            "{beautiful |}woman, at a {lake|pond}, {skipping stones|rowing a boat}, playful grin, close-up",
            "hawaiian {woman|man}, {beach|garden} selfie, {flower crown|leis}, {smiling|goofy} expression",
            "{beautiful |cute |}{norwegian|swedish} college girl, selfie on the couch, {braided hair|ponytail}, puckered lips",
            "{beautiful |cute |}indian {man|woman}, {study|home office} selfie, {spectacles|headphones}, amused",
            "{beautiful |cute |}{japanese|korean} {man|woman}, {bathroom|bedroom} selfie, {casual wear|pajamas}, dimpled grin",
            "{beautiful |cute |}african {woman|man}, {balcony|terrace} full-body selfie, {sundress|shorts}, {barefoot|sandaled} with a {laid-back|cheery} smile",
            "{beautiful |cute |}{polish|ukrainian} girl, {bedroom|living room} mirror selfie, {tracksuit|leisure wear}, {tied up hair|cap}, {casual|relaxed} look",
            "{beautiful |cute |}{mexican|spanish} {man|woman}, {couch|garden} selfie, {book|coffee} in hand, {glasses|beanie}, {content|absorbed} expression",
            "{beautiful |cute |}chinese {woman|man}, {kitchen|dining room} selfie, {apron|chef’s hat}, {cooking|baking}, {pleased|excited} grin",
            "{beautiful |cute |}{italian|french} {woman|man}, {bedroom|home studio} mirror selfie, {paint-stained jeans|smudged shirt}, {brush|palette} in hand, {creative|inspired} look",
            "{beautiful |cute |}{canadian|american} {man|woman}, {backyard|front porch} full-body selfie, {plaid shirt|denim jacket}, {baseball cap|sunglasses}, {grinning|winking} expression",
            "{mountain|Mediterranean} vista, {sunrise|sunset}, {forested|clear} landscape, {misty|clear} view",
            "cityscape, {night|dawn}, {skyscrapers|historic buildings}, {glistening|soft} lights",
            "{safari|grassland}, {lion|elephant}, {prowling|grazing}, {hot midday|cool dusk}",
            "{celtic|scottish|irish} man, early {30s|40s}, {red|auburn|ginger} hair, wearing a {tartan|woolen|clan} kilt, on a {windy|grass-covered|rolling} hill, {bagpipes|flute} in the background, close up",
            "{hawaiian|polynesian|tahitian} woman, late {teens|early 20s}, {long|curly|flowing} hair, wearing a {grass|leaf|floral} skirt, on the {beach|seashore}, {hula|traditional} dance, close up",
            "{mexican|texan|new mexican} man, mid {40s|50s}, {mustache|beard}, wearing a {sombrero|wide-brimmed hat}, {zarape|poncho}, in a {sleepy|humble|quiet} village, {siesta time|afternoon nap}, close up",
            "{indonesian|javanesian|balinese} woman, early {30s|40s}, {batik|floral|traditional} headscarf, in a {rice paddy|green field}, {distant volcano|mountain range}, {sunset|dusk}, close up",
            "{australian|aussie|outback} man, late {20s|30s}, {sun-bleached|light brown|sandy} hair, wearing a {bush|leather|outback} hat, in a {dusty outback|rugged wilderness}, {kangaroo|hopping kangaroo|wildlife} hopping by, close up",
            "{cambodian|khmer|south asian} woman, mid {20s|30s}, {sleek|dark|long} hair, wearing a {sampot|traditional dress}, in a {temple|ancient ruin}, {incense|candle} smoke, close up",
            "{argentinian|chilean|south american} man, early {30s|40s}, {slicked-back|neat|tidy} hair, wearing a {gaucho|cowboy} outfit, on a {ranch|farm}, {horses|livestock} grazing, close up",
            "{ecuadorian|peruvian|andes} woman, late {teens|early 20s}, wearing a {colorful|vibrant|woolen} poncho, in a {flower|outdoor} market, {Andean|mountainous} mountains, close up",
            "{romanian|transylvanian|east european} woman, early {40s|30s}, {braided|woven|long} hair, wearing a {peasant blouse|traditional top}, in a {fortified church|ancient chapel}, {medieval|rustic} atmosphere, close up",
            "{samoan|polynesian|pacific} man, early {30s|40s}, {muscular|strong|well-built}, wearing a {lava-lava|traditional cloth}, on a {beach|shore}, {Pacific Ocean|lagoon}, {sunset|dusk}, close up",
            "{scottish|highland|celtic} woman, late {20s|early 30s}, {red|strawberry blonde|auburn} curls, wearing a {tartan|woolen|plaid} shawl, on a {windy|lonely|bare} moor, {bagpipes|lonely music} in the distance, close up",
            "{peruvian|andes|south american} man, late {40s|50s}, wearing a {chullo|woolen cap}, in a {market|bazaar}, {llamas|alpacas} grazing, close up",
            "{vietnamese|southeast asian|asiatic} woman, {20s|late teens}, {long|flowing|black} hair, wearing an {ao dai|traditional dress}, in a {lantern-lit|moonlit} street, {Tet celebration|New Year celebration}, close up",
            "{cyberpunk|futuristic|punk} girl, {neon|flaming|luminous} hair, {augmented reality glasses|holographic visor|laser monocle}, in a {technopolis|megacity|electron city}, {neon lights|laser beams|holographic billboards}, {graffiti|poster-covered|LED} walls, close up",
            "{ninja|samurai|assassin}, {masked|hooded|hooded and masked}, {darting|piercing|alert} eyes, {shuriken|kunai|ninja stars} on belt, lurking in the {shadows|darkness|underbrush}, {bamboo|pine|sakura} forest, {full|crescent|new} moon, close up",
            "{succubus|demoness|fiend}, {crimson|obsidian|raven} hair, {black|dark|shadowy} wings, {revealing|sculpted|intricate} black dress, in a {gothic|ancient|forbidding} castle, {flickering|dim|sputtering} candles",
            "{greek|roman|mythical} goddess, {flowing|draping|billowing} white robes, {golden|silver|bronze} laurel crown, in an {ancient|marble|stone} temple, {olive|cypress|palm} trees, {setting|rising|midday} sun, close up",
            "{steampunk|vintage|retro} woman, {goggles|monocle|spectacles}, {corset|bodice|vest}, in a {Victorian|industrial-era|old-world} cityscape, {steam-powered|gear-driven|clockwork} machines, {cobblestone|brick-paved|gravel} streets, close up",
            "{muscular|towering|burly} tauren, {tribal|ancient|spiritual} tattoos, wearing a {fur kilt|leather loincloth|hide pants}, in a {grassy knoll|sacred site|prairie}, {towering|carved|ancient} totem poles, {sunset|dawn|twilight}",
            "{attractive|mysterious|alluring} vampire, {pale|moonlit|translucent} skin, {slicked back|messy|curly} hair, {piercing|glowing|shimmering} red eyes, in a {European|medieval|ancient} castle, {full|crescent|cloud-covered} moon, close up",
            "{viking|norse|warrior} warrior, {burly|muscular|battle-hardened}, {spear|axe|sword}, wearing a {fur cloak|leather armor|wolf pelt}, in a {longboat|drakkar|ship}, {icy fjord|churning sea|calm lake}, {northern lights|starry sky|misty skies}, close up",
            "{tiefling|demonic|hellborn} priestess, {dark purple|burgundy|deep blue} skin, {long|curved|twisted} horns, wearing a {ceremonial|sacred|ornate} robe, in a {darkened|shadowy|silent} chapel, {stained glass|mosaic|etched} windows",
            "{ogre|giant|brute}, {large|imposing|towering}, wearing {rough hide|patchwork leather|thick fur} armor, in a {dank|murky|fetid} swamp, {buzzing|swarming} mosquitoes",
            "{fairy|fae|pixie}, {iridescent|shimmering|translucent} wings, {long wavy|curly|pixie cut} hair, in a {mushroom|fairy|flower} ring, {forest|glen|meadow} glade, {fireflies|glow worms|moonbeams} around, close up",
            "{mermaid|siren}, {shimmering|sparkling|radiant} tail, {flowing|curly|wavy} red hair, {sunning|lounging|resting} on a rock, {azure|crystal-clear|turquoise} sea, {seagulls|albatrosses|pelicans} soaring, close up",
            "{dragonborn|draconian|drake-blooded}, {scales|spines|ridged armor}, wearing a {plate|chainmail|scale} armor, in a {cavern|mountain lair|hidden den}, hoard of {treasure|gold|jewels}, {flickering|blazing|guttering} torchlight, close up",
            "{elven|sylvan|woodland} ranger, {longbow|shortbow|crossbow}, {green|brown|forest} cloak, in a {dense|lush|deep} forest, {ancient|majestic|towering} trees, {rustling|whispering|swaying} leaves, close up",
            "{gnome|dwarf|halfling} tinkerer, {goggles|spectacles|monocle}, {overalls|workwear|apron}, in a {cluttered|chaotic|bustling} workshop, {gears and springs|cogs and wheels|parts and pieces}, {sparks|flames|embers} flying, close up",
            "{amazonian|warrior|jungle} warrior, {tall|statuesque|mighty} and muscular, wearing a {leather cuirass|scale armor|cloth wrap}, in a {jungle|rainforest|canopy} clearing, {parrots|macaws|toucans} squawking, close up",
            "{cybernetic|futuristic|techno} samurai, {katana|laser sword|monomolecular blade}, in a {neo-Tokyo|dystopian|futuristic} cityscape, {neon signs|holo ads|LED billboards}, {rain-soaked|neon-lit|foggy} streets, close up",
            "{sorceress|mage|witch}, {staff|wand|orb}, in a {spellbound|enchanted|arcane} tower, {floating|levitating|hovering} books, {arcane|mystic|ancient} symbols, close up",
        ];

        private async Task BeginGenerate()
        {
            config.EndEdit();
            cfg.Save();

            SuspendLayout();

            for (int i = 0; i < clients.Count; i++)
            {
                if (i < cfg.Count)
                    clients[i].BeginGenerate();
                else
                    clients[i].Visible = false;
            }

            btnCancel.Enabled = true;
            btnGenerate.Enabled = false;
            btnRandomGen.Enabled = false;
            cboArtStyle.Enabled = false;
            tbrCount.Enabled = false;
            txtDescription.Enabled = false;
            txtNegative.Enabled = false;
            btnLandscape.Enabled = false;
            btnPortrait.Enabled = false;
            btnSquare.Enabled = false;
            tbrGuidanceScale.Enabled = false;
            panHistory.Enabled = false;
            btnVietnamese.Enabled = false;
            btnEnglish.Enabled = false;

            if (cfg.Language != "en")
            {
                cfg.Description = await TranslateText(cfg.RawDescription, cfg.Language);
                cfg.Negative = await TranslateText(cfg.RawNegative, cfg.Language);
            }
            else
            {
                cfg.Description = cfg.RawDescription;
                cfg.Negative = cfg.RawNegative;
            }

            ResumeLayout();
        }

        private void EndGenerate()
        {
            btnCancel.Enabled = false;
            btnGenerate.Enabled = true;
            foreach (var cli in clients)
                cli.EndGenerate();
            btnGenerate.Enabled = true;
            btnRandomGen.Enabled = true;
            cboArtStyle.Enabled = true;
            tbrCount.Enabled = true;
            txtDescription.Enabled = true;
            txtNegative.Enabled = true;
            btnLandscape.Enabled = true;
            btnPortrait.Enabled = true;
            btnSquare.Enabled = true;
            tbrGuidanceScale.Enabled = true;
            btnVietnamese.Enabled = true;
            btnEnglish.Enabled = true;

            LoadHistory();

            panHistory.Enabled = true;
        }

        static async Task<string> TranslateText(string text, string from, string to = "en")
        {
            const string api = "https://script.google.com/macros/s/AKfycbyS4gkT4LqxDjjuAdKnXPDL43jfECYXu_uz7bMLqdaLasuqsehdSNtGvZYVAfQXpAPCmg";

            using var cli = new HttpClient();
            var req = $"{api}/exec?text={Uri.EscapeDataString(text)}&from={from}&to={to}";
            var res = await cli.GetAsync(req, Global.Cancellation.Token);
            if (res.IsSuccessStatusCode)
                return await res.Content.ReadAsStringAsync(Global.Cancellation.Token);
            return text;
        }

        public FrmMain()
        {
            InitializeComponent();

            cboArtStyle.Items.AddRange(styles.Keys.Cast<object>().ToArray());

            config.Add(cfg);
            btnEnglish.Checked = cfg.Language == "en";
            btnVietnamese.Checked = cfg.Language == "vi";

            LoadHistory();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    btnCancel.PerformClick();
                    return true;
                case Keys.F5:
                    btnGenerate.PerformClick();
                    return true;
                case Keys.F6:
                    btnRandomGen.PerformClick();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            tlpLayout.SuspendLayout();

            for (var i = 0; i < tbrCount.Maximum; i++)
            {
                var cli = new PerchanceBox
                {
                    Thread = i,
                    Dock = DockStyle.Fill,
                    Visible = false,
                    Size = new Size(100, 480)
                };
                tlpLayout.Controls.Add(cli, i % 3, i / 3);
                clients.Add(cli);
            }

            tlpLayout.ResumeLayout();
        }

        static Image? GetIcon(string folderPath, int width = 48, int height = 48)
        {
            try
            {
                var directory = new DirectoryInfo(folderPath);
                var latestFile = directory.GetFiles()
                    .Where(f => f.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                    .MaxBy(f => f.CreationTime);

                if (latestFile == null)
                    return null;

                using var originalImage = Image.FromFile(latestFile.FullName);
                var resizedImage = new Bitmap(width, height);
                using var graphics = Graphics.FromImage(resizedImage);
                graphics.Clear(Color.White);

                var scale = Math.Min((float)width / originalImage.Width, (float)height / originalImage.Height);
                var scaledWidth = (int)(originalImage.Width * scale);
                var scaledHeight = (int)(originalImage.Height * scale);

                var offsetX = (width - scaledWidth) / 2;
                var offsetY = (height - scaledHeight) / 2;

                graphics.DrawImage(originalImage, offsetX, offsetY, scaledWidth, scaledHeight);
                graphics.Save();

                return resizedImage;
            }
            catch
            {
                return null;
            }
        }

        private void LoadHistory()
        {
            lvHistory.Items.Clear();

            var filter = txtFilter.Text;
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var lst = new List<Configuration>();
            foreach (var d in Directory.EnumerateDirectories(dir))
                lst.Add(Configuration.Load(d));
            var q = btnOldFirst.Checked ? lst.OrderBy(x => x.CreatedDate) : lst.OrderByDescending(x => x.CreatedDate);
            foreach (var c in q.Where(x => x.RawDescription.Contains(filter, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (ilHistory.Images.ContainsKey(c.Hash))
                    ilHistory.Images.RemoveByKey(c.Hash);
                ilHistory.Images.Add(c.Hash, GetIcon(Path.Combine(dir, c.Hash)) ?? ilHistory.Images["NO_IMAGE"]!);
                var i = lvHistory.Items.Add(c.Hash, c.ArtStyle + "\r\n" + c.RawDescription, c.Hash);
                i.Tag = c;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            Global.Cancellation = new CancellationTokenSource();

            await BeginGenerate();

            try
            {
                var lst = new List<Task>();
                for (var i = 0; i < clients.Count; i++)
                    if (i < cfg.Count)
                    {
                        await Global.Semaphore.WaitAsync();

                        var t = clients[i].Generate(cfg);
                        lst.Add(t);
                    }
                foreach (var t in lst)
                    await t;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            EndGenerate();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Global.Cancellation!.Cancel();
        }

        private void btnRandomGen_Click(object sender, EventArgs e)
        {
            var text = suggestion[rand.Next(suggestion.Length)];
            text = ImportRegex().Replace(text, m =>
            {
                var name = m.Groups[1].Value;
                return repl.TryGetValue(name, out var value) ? value[rand.Next(value.Count)] : name;
            });
            text = ChooseRegex().Replace(text, m =>
            {
                var data = m.Groups[1].Value.Split('|');
                return data[rand.Next(data.Length)];
            });
            txtDescription.Text = text;
            btnGenerate.PerformClick();
        }

        private void btnEnglish_CheckedChanged(object sender, EventArgs e)
        {
            if (btnEnglish.Checked)
            {
                cfg.Language = "en";
                btnVietnamese.Checked = false;
            }
        }

        private void btnVietnamese_CheckedChanged(object sender, EventArgs e)
        {
            if (btnVietnamese.Checked)
            {
                cfg.Language = "vi";
                btnEnglish.Checked = false;
            }
        }

        private void btnPortrait_Click(object sender, EventArgs e)
        {
            txtWidth.Text = "512";
            txtHeight.Text = "768";
        }

        private void btnLandscape_Click(object sender, EventArgs e)
        {
            txtWidth.Text = "768";
            txtHeight.Text = "512";
        }

        private void btnSquare_Click(object sender, EventArgs e)
        {
            txtWidth.Text = "512";
            txtHeight.Text = "512";
        }

        private void lvHistory_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var info = lvHistory.HitTest(e.X, e.Y);
            if (info.Item is not { Tag: Configuration c })
                return;

            cfg.CopyFrom(c);
            config.ResetBindings(false);

            var i = 0;
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history", c.Hash);
            if (!Directory.Exists(dir))
                return;

            tlpLayout.SuspendLayout();

            foreach (var cli in clients)
                cli.Visible = false;

            foreach (var d in Directory.GetFiles(dir, "*.jpg"))
                if (i < clients.Count)
                    clients[i].LoadImage(d, () => ++i);
                else
                    break;

            tlpLayout.ResumeLayout();
        }

        private void btnNewFirst_CheckedChanged(object sender, EventArgs e)
        {
            if (btnNewFirst.Checked)
            {
                btnOldFirst.Checked = false;
                LoadHistory();
            }
        }

        private void btnOldFirst_CheckedChanged(object sender, EventArgs e)
        {
            if (btnOldFirst.Checked)
            {
                btnNewFirst.Checked = false;
                LoadHistory();
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            tmrSearch.Stop();
            tmrSearch.Start();
        }

        private void tmrSearch_Tick(object sender, EventArgs e)
        {
            tmrSearch.Stop();
            LoadHistory();
        }

        private void btnRefreshHistory_Click(object sender, EventArgs e)
        {
            LoadHistory();
        }

        private static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
                return;

            foreach (var dir in baseDir.EnumerateDirectories())
                RecursiveDelete(dir);
            var files = baseDir.GetFiles();
            foreach (var file in files)
            {
                file.IsReadOnly = false;
                file.Delete();
            }
            baseDir.Delete();
        }

        private readonly Func<bool> ConfirmDelete = () => MessageBox.Show("Are you sure to delete selected history?",
            "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        private void DeleteSelectedHistory()
        {
            if (!ConfirmDelete())
                return;

            foreach (ListViewItem item in lvHistory.SelectedItems)
                if (item.Tag is Configuration c)
                {
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history", c.Hash);
                    RecursiveDelete(new DirectoryInfo(dir));
                }
            LoadHistory();
        }

        private void lvHistory_KeyUp(object sender, KeyEventArgs e)
        {
            if (e is { KeyCode: Keys.Delete, Modifiers: Keys.None })
                DeleteSelectedHistory();
            else if (e is { KeyCode: Keys.A, Modifiers: Keys.Control })
                foreach (ListViewItem item in lvHistory.Items)
                    item.Selected = true;
            else if (e is { KeyCode: Keys.Escape, Modifiers: Keys.None })
                foreach (ListViewItem item in lvHistory.Items)
                    item.Selected = false;
        }

        private void mniOpenFolder_Click(object sender, EventArgs e)
        {
            if (cmsHistory.Tag is Configuration c)
                Process.Start(new ProcessStartInfo("explorer.exe", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history", c.Hash))
                {
                    UseShellExecute = true,
                    Verb = "open"
                });
        }

        private void mniDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedHistory();
        }

        private void mniRefresh_Click(object sender, EventArgs e)
        {
            LoadHistory();
        }

        private void lvHistory_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            var info = lvHistory.HitTest(e.X, e.Y);
            if (info.Item is { Tag: Configuration c })
            {
                mniDelete.Enabled = true;
                mniOpenFolder.Enabled = true;
                cmsHistory.Tag = c;
            }
            else
            {
                mniDelete.Enabled = false;
                mniOpenFolder.Enabled = false;
            }
            cmsHistory.Show(lvHistory, e.X, e.Y);
        }

        private void mniCopyPath_Click(object sender, EventArgs e)
        {
            if (cmsHistory.Tag is Configuration c)
            {
                Clipboard.SetText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history", c.Hash));

                Toast.Show("Path copied !");
            }
        }
    }
}
