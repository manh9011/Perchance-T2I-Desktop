using System.Text.RegularExpressions;

namespace Perchance
{
    public class ArtStyle(Func<Configuration, string> prompt, Func<Configuration, string> negative)
    {
        public static readonly Random rand = new((int)(DateTime.Now.Ticks % int.MaxValue));
        public static string Choose(params string[] args) => args.Length > 0 ? args[rand.Next(args.Length)] : "";

        public static readonly Dictionary<string, ArtStyle> styles = new()
        {
            ["No Style"] = new(
                prompt: input => input.Description,
                negative: input => input.Negative
            ),

            ["Painted Anime"] = new(
                prompt: input => $"{Regex.Replace(input.Description, @"\b(male|man)\b", "(male, masculine, masc, male)", RegexOptions.IgnoreCase)},\r\nart by atey ghailan, painterly anime style at pixiv, art by kantoku, in art style of redjuice/necömi/rella/tiv pixiv collab,\r\nyour name anime art style, masterpiece digital painting, exquisite lighting and composition, inspired by wlop art style, 8k,\r\nsharp, very detailed, high resolution, illustration ^2 painterly anime artwork, {input.Description}, masterpiece, fine details, breathtaking\r\nartwork, painterly art style, high quality, 8k, very detailed, high resolution, exquisite composition and lighting",
                negative: input => $"(worst quality, low quality, blurry:1.3), {input.Negative}, low-quality, deformed, text, poorly drawn\r\n{(Regex.IsMatch(input.Description, "(male|man)", RegexOptions.IgnoreCase) ? ", female, feminine, fem, female" : "")} {input.Negative}, (worst quality,\r\nlow quality, blurry:1.3), black and white, low-quality, deformed, text, poorly drawn, bad art, bad anatomy, bad lighting,\r\ndisfigured, faded, blurred face"
            ),

            ["Casual Photo"] = new(
                prompt: input => $"{input.Description}, casual photo",
                negative: input => $"{input.Negative}, bad photo, bad lighting, high production value, unnatural studio lighting, commercial photoshoot, photoshopped, terrible photo, disfigured"
            ),

            ["Cinematic"] = new(
                prompt: input => $"{input.Description}, cinematic shot, dynamic lighting, 75mm, Technicolor, Panavision, cinemascope, sharp focus, fine\r\ndetails, 8k, HDR, realism, realistic, key visual, film still, cinematic color grading, depth of field",
                negative: input => $"{input.Negative}, bad lighting, low-quality, deformed, text, poorly drawn, holding camera, bad art, bad angle,\r\nboring, low-resolution, worst quality, bad composition, disfigured"
            ),

            ["Digital Painting"] = new(
                prompt: input => $"{input.Description}, breathtaking digital art, trending on artstation, by atey ghailan, by greg rutkowski, by greg tocchini, by james gilleard, 8k, high resolution, best quality",
                negative: input => $"{input.Negative}, low-quality, deformed, signature watermark text, poorly drawn"
            ),

            ["Concept Art"] = new(
                prompt: input => $"{input.Description}, concept art, digital art, illustration, inspired by wlop style, 8k, fine details, sharp, very detailed, high resolution, masterpiece",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn, worst quality, blurry"
            ),

            ["3D Disney Character"] = new(
                prompt: input => $"3D cartoon Disney character portrait render. {input.Description}, bokeh, 4k, highly detailed, Pixar render, CGI Animation, Disney,\r\ncute big circular reflective eyes, dof, (cinematic film), Disney realism, subtle details, breathtaking Pixar short, fine details,\r\nclose up, sharp focus, HDR, Disney-style octane render, incredible composition, superb lighting and detail, {input.Description}",
                negative: input => $"{input.Negative}, worst quality, poorly drawn, bad art, boring, deformed, bad composition, crappy artwork, bad lighting"
            ),

            ["2D Disney Character"] = new(
                prompt: input => $"2D cartoon Disney character digital art of {input.Description}. superb linework, classic 2D Disney style art, close-up, inspired by the art styles of Glen Keane and Aaron Blaise,\r\nDisney-style character concept with a Disney-style face, (trending on artstation), Disney-style version of {input.Description}",
                negative: input => $"{input.Negative}, bad 3D render, bad 3D shadowing, worst quality, poorly drawn, low-resolution render, bad colors, a photo, terrible art, text, logo, bad composition,\r\nbad lighting, disfigured, deformed, bad anatomy, crappy educational infographic cartoon, bad 3D render, black and white photo"
            ),

            ["Disney Sketch"] = new(
                prompt: input => $"Glen Keane character concept art (black and white) pencil sketch of {input.Description}. (rough pencil sketch by Glen Keane:1.2), rough pencil sketch, close up,\r\nloose Disney-style character concept art sketch, (nice sketchy pencil strokes), Disney character design sketch, pencil texture, a concept art pencil sketch of {input.Description},\r\n(by Glen Keane:1.3)",
                negative: input => $"{input.Negative}, a photo, photorealistic style, with color, (a bad photo), bad digital art, bad 3D render, gradient, overly detailed, high contrast, bloom,\r\na color photo, smooth shading, hilariously bad drawings, bad anatomy, bad art, 3D, a photo, soft shadows, lot of color"
            ),

            ["Concept Sketch"] = new(
                prompt: input => $"black and white technical drawing showcasing a {input.Description}, annotation details, masterpiece black and white, pencil strokes, annotated technical concept art sketch, pencil texture",
                negative: input => input.Negative
            ),

            ["Painterly"] = new(
                prompt: input => $"painterly digital painting, {input.Description}, digital painting by Ilya Kuvshinov with painterly brush strokes, by Ilya Kuvshinov, painterly masterpiece",
                negative: input => $"brush, {input.Negative}, bad 3D render, (holding paintbrush), easel, bad photo, bad art, boring, bad 3D render"
            ),

            ["Oil Painting"] = new(
                prompt: input => $"breathtaking alla prima oil painting, {input.Description}, close up, (alla prima style:1.3), oil on linen, painterly oil on canvas, (painterly style:1.3),\r\nexquisite composition and lighting, modern painterly masterpiece, by alexi zaitsev, award-winning painterly alla prima oil painting",
                negative: input => $"{input.Negative}, framed, faded colors, terrible photo, bad composition, hilariously bad drawing, bad photo, bad anatomy, extremely high contrast, worst quality, watermarked signature,\r\nbad colors, deformed, amputee, washed out, glare, boring colors, bad crayon drawing"
            ),

            ["Oil Painting - Realism"] = new(
                prompt: input => $"breathtaking oil painting, {input.Description}, photorealistic oil painting, by charlie bowater, fine details, by wlop, trending on artstation, very detailed",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn, worst quality"
            ),

            ["Oil Painting - Old"] = new(
                prompt: input => $"({input.Description}) (Oil painting) (by Jean-François Millet), (by Gustave Courbet), (by Jules Breton), close up",
                negative: input => $"{input.Negative}, frame, (badly drawn hands), extra limbs, extra fingers, bad anatomy"
            ),

            ["Professional Photo"] = new(
                prompt: input => $"{input.Description}, {Choose("sharp", "soft")} focus, depth of field, 8k photo, HDR, professional lighting, taken with Canon EOS R5, 75mm lens",
                negative: input => $"{input.Negative}, worst quality, bad lighting, cropped, blurry, low-quality, deformed, text, poorly drawn, bad art, bad angle, boring, low-resolution, worst quality, bad composition,\r\nterrible lighting, bad anatomy"
            ),

            ["Anime"] = new(
                prompt: input => $"(anime art of {input.Description}:1.2), masterpiece, 4k, best quality, anime art",
                negative: input => $"(worst quality, low quality:1.3), {input.Negative}, low-quality, deformed, text, poorly drawn, hilariously bad drawing, bad 3D render"
            ),

            ["Drawn Anime"] = new(
                prompt: input => $"digital art drawing, illustration of ({input.Description}), anime drawing/art, bold linework, illustration, cel shaded, painterly style, digital art, masterpiece",
                negative: input => $"{input.Negative}, boring flat infographic, oversaturated, bad photo, terrible 3D render, bad anatomy, worst quality, greyscale, black and white, disfigured, deformed, glitch, cross-eyed,\r\nlazy eye, ugly, deformed, distorted, glitched, lifeless, low quality, bad proportions"
            ),

            ["Cute Anime"] = new(
                prompt: input => $"(((adorable, cute, kawaii)), {input.Description}, cute moe anime character portrait, adorable, featured on pixiv, kawaii moé masterpiece, cuteness overload, very detailed, sooooo adorable!!!,\r\nabsolute masterpiece",
                negative: input => $"(worst quality, low quality:1.3), {input.Negative}, worst quality, ugly, 3D, photograph, bad art, blurry, boring"
            ),

            ["Soft Anime"] = new(
                prompt: input => $"{input.Description}, anime masterpiece, soft lighting, intricate, highly detailed, pixiv, anime art, 4k, art from your name anime, garden of words style art, high quality",
                negative: input => $"(worst quality, low quality:1.3), {input.Negative}, low-quality, deformed, text, poorly drawn"
            ),

            ["Fantasy Painting"] = new(
                prompt: input => $"{input.Description}, d&d, fantasy, highly detailed, digital painting, artstation, sharp focus, fantasy art, illustration, 8k, in the style of greg rutkowski",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn"
            ),

            ["Fantasy Landscape"] = new(
                prompt: input => $"{input.Description}, fantasy matte painting, absolute masterpiece, detailed matte painting by andreas rocha and greg rutkowski, by Brothers Hildebrandt, superb composition, vivid fantasy art,\r\nbreathtaking fantasy masterpiece",
                negative: input => $"{input.Negative}, faded, blurry, bad art, boring"
            ),

            ["Fantasy Portrait"] = new(
                prompt: input => $"{input.Description}, d&d, fantasy, highly detailed, digital painting, artstation, sharp focus, fantasy art, character art, illustration, 8k, art by artgerm and greg rutkowski",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn"
            ),

            ["Studio Ghibli"] = new(
                prompt: input => $"{input.Description}, (studio ghibli style art:1.3), sharp, very detailed, high resolution, inspired by hayao miyazaki, anime, art from ghibli movie",
                negative: input => $"(worst quality, low quality:1.3), {input.Negative}, low-quality, deformed, text, poorly drawn"
            ),

            ["50s Enamel Sign"] = new(
                prompt: input => $"50s enamel sign of {input.Description}, 50s advert enamel sign, masterpiece, authentic vintage enamel sign",
                negative: input => $"{input.Negative}"
            ),

            ["Vintage Comic"] = new(
                prompt: input => $"comic book style art of {input.Description}, (drawing, by Dave Stevens, by Adam Hughes, 1940's, 1950's:1.2), hand-drawn, color, high resolution, best quality, closeup",
                negative: input => $"{input.Negative}, terrible photoshop, low contrast, disfigured, poorly drawn, deformed, tiled, cropped, framed"
            ),

            ["Franco-Belgian Comic"] = new(
                prompt: input => $"franco-belgian color comic about {input.Description}, bande dessinée, franco-belgian comic panel, masterpiece, breathtaking composition, intricate, detailed, best quality, close-up",
                negative: input => $"{input.Negative}, framed blurry crappy photo, overexposed, worst quality, border, deformed horror, overly faded"
            ),

            ["Tintin Comic"] = new(
                prompt: input => $"color comic panel in the style of Hergé about {input.Description}, by Hergé, tintin style, french comic panel, franco-belgian style, close-up, masterpiece, high-resolution Hergé style",
                negative: input => $"{input.Negative}, blurry, framed edge border, text, overexposed, bad anatomy, deformed, disfigured, blur, horror, dead eyes, boring, faded, (worst quality, low quality:1.2)"
            ),

            ["Medieval"] = new(
                prompt: input => $"medieval illuminated manuscript picture of {input.Description}, medieval illuminated manuscript art, masterpiece medieval color illustration, 16th century, 8k high-resolution scan of 16th century illuminated manuscript painting, detailed medieval masterpiece, close-up",
                negative: input => $"{input.Negative}, worst quality, blurry"
            ),

            ["Pixel Art"] = new(
                prompt: input => $"{(input.Description.Length > 40 ? "(pixel art), " : "")}{input.Description}, best pixel art, neo-geo graphical style, retro nostalgic masterpiece, 128px, 16-bit pixel art [input.description.length < 10 ? \"of \"+input.description : \"\"], 2D pixel art style, adventure game pixel art, inspired by the art style of hyper light drifter, masterful dithering, superb composition, beautiful palette, exquisite pixel detail",
                negative: input => $"{input.Negative}, glitched, deep fried, jpeg artifacts, out of focus, gradient, soft focus, low quality, poorly drawn, blur, grainy fabric texture, text, bad art, boring colors, blurry platformer screenshot"
            ),

            ["Furry - Oil"] = new(
                prompt: input => $"({input.Description}:1.3), crisp vibrant detailed soft painterly digital art, volumetric lighting, natural lighting, realistic lighting, vibrant colors, crisp oil painting, painterly realism, depth of field, subtle soft details, vivid, fresh, striking, by chunie, by darkgem, by honovy, by zaush, by anhes, by puinkey, by caraid, by dagasi, by taranfiddler, by atey ghailan, by MilletGustave, by Curbet, Charlie Bowater, lol art",
                negative: input => $"{input.Negative}, blurry, faded, antique, muted colors, greyscale, boring colors, flat, bad photo, terrible 3D render, bad anatomy, worst quality, greyscale, black and white, disfigured, deformed, glitch, cross-eyed, lazy eye, ugly, deformed, distorted, glitched, lifeless, low quality, bad proportions, watermark, signature"
            ),

            ["Furry - Cinematic"] = new(
                prompt: input => $"{input.Description}, cinematic shot, dynamic lighting, 75mm, Technicolor, Panavision, cinemascope, sharp focus, fine details, 8k, HDR, realism, realistic, key visual, film still, cinematic color grading, depth of field, (anthro:0.1)",
                negative: input => $"{input.Negative}, bad lighting, low-quality, deformed, text, poorly drawn, holding camera, bad art, bad angle, boring, low-resolution, worst quality, bad composition, disfigured"
            ),

            ["Furry - Painted"] = new(
                prompt: input => $"anthro {input.Description} digital art, masterpiece, 4k, fine details",
                negative: input => $"{input.Negative}, bad photo, worst quality, bad composition, bad lighting, bad colors, small eyes, low quality, bad art, poorly drawn, deformed, bad 3D render, boring, lifeless, deformed, ugly, low resolution"
            ),

            ["Furry - Drawn"] = new(
                prompt: input => $"anthro {input.Description} illustration, hand-drawn, bold linework, anthro illustration, cel shaded, 4k, fine details, masterpiece",
                negative: input => $"{input.Negative}, bad photo, terrible 3D render, bad anatomy, worst quality, greyscale, black and white, disfigured, deformed, glitch, cross-eyed, lazy eye, ugly, deformed, distorted, glitched, lifeless, low quality, bad proportions"
            ),

            ["Cute Figurine"] = new(
                prompt: input => $"{input.Description}, figurine, modern Disney style, octane render, chibi",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn"
            ),

            ["3D Emoji"] = new(
                prompt: input => $"masterpiece (({input.Description})) cartoon emoji concept render, (close-up:1.3), facing forward, (matte), emoji render trending on artstation, noto color emoji, app icon, joypixels, simple design, new iOS 16.4 ((({input.Description}))) emoji render, (simple background:1.2), (centered:1.2), masterpiece, telegram sticker, clash of clans character concept, (looking at camera), crisp render, sharp focus, simple cartoon design, 4k, sharp focus, an emoji with cartoon proportions, masterpiece emoji-style figurine render, transparent background png",
                negative: input => $"{input.Negative}, framed, inset, border, glare, blurry, out of focus, shiny, (on pedestal:1.2), on platform, with base, gradient background, off-center, casting shadow, ((deformed, disfigured)), glitchy, vector art, text, signature, bad anatomy, messed up, bad art, gradient background, complex background, blurry, worst quality, low resolution, messy, complex, gradient background, bad lighting"
            ),

            ["Illustration"] = new(
                prompt: input => $"breathtaking illustration of {input.Description}, (illustration:1.3), masterpiece, breathtaking illustration",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn, bad 3D render, bad composition, bad photo, worst quality"
            ),

            ["Flat Illustration"] = new(
                prompt: input => $"{input.Description}, illustration, flat, 2D, vector art, masterpiece, made with adobe illustrator, behance competition winner, trending on dribble, 4k, high resolution, crisp lines",
                negative: input => $"{input.Negative}, bad art, children's crayon drawing, worst quality, blurry, blur, jpeg artifacts, compression artifacts, text, worst quality, noise, messy, low resolution"
            ),

            ["Watercolor"] = new(
                prompt: input => $"{input.Description}, (watercolor), high resolution, intricate details, 4k, wallpaper, concept art, watercolor on textured paper",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn"
            ),

            ["1990s Photo"] = new(
                prompt: input => $"{input.Description}, 90s home video, nostalgic 90s photo, taken with kodak disposable camera",
                negative: input => $"{input.Negative}, blurry, blur, deformed"
            ),

            ["1980s Photo"] = new(
                prompt: input => $"famous vintage 80s photo, {input.Description}, grainy photograph, 80s photo with film grain, Kodacolor II 80s photo with vignetting, retro, r/OldSchoolCool, 80s photo with wear and tear and minor creasing and scratches, vintage color photo",
                negative: input => $"{input.Negative}, deformed"
            ),

            ["1970s Photo"] = new(
                prompt: input => $"famous vintage 70s photo, {input.Description}, grainy photograph, 1970s photo with film grain, 70s photo with vignetting, retro, r/OldSchoolCool, 70s photo, vintage photo",
                negative: input => $"{input.Negative}, deformed"
            ),

            ["1960s Photo"] = new(
                prompt: input => $"famous vintage 60s photo, {input.Description}, grainy photograph, 1960s photo with film grain, 60s photo with vignetting, retro, r/OldSchoolCool, 60s photo, vintage photo",
                negative: input => $"{input.Negative}, deformed"
            ),

            ["1950s Photo"] = new(
                prompt: input => $"famous vintage 50s photo, {input.Description}, grainy photograph, 1950s photo with film grain, 1950s photo with vignetting, retro, r/OldSchoolCool, 1950s photo, vintage photo",
                negative: input => $"{input.Negative}, deformed"
            ),

            ["1940s Photo"] = new(
                prompt: input => $"famous vintage 1940s photo, {input.Description}, grainy photograph, 1940s photo with film grain, 1940s photo with vignetting, retro, r/OldSchoolCool, 1940s photo, vintage photo",
                negative: input => $"{input.Negative}, deformed"
            ),

            ["1930s Photo"] = new(
                prompt: input => $"famous vintage 1930s photo, {input.Description}, grainy photograph, 1930s photo with film grain, 1930s photo with vignetting, retro, r/OldSchoolCool, 1930s photo, vintage photo",
                negative: input => $"{input.Negative}, deformed"
            ),

            ["1920s Photo"] = new(
                prompt: input => $"famous vintage 1920s photo, {input.Description}, 1920s photo, restored photograph, {Choose("sepia", "black and white")}, historical archive photo",
                negative: input => $"{input.Negative}, deformed"
            ),

            ["Vintage Pulp Art"] = new(
                prompt: input => $"(((1970s vintage pulp art))), {input.Description}, vintage pulp art, by Earle K. Bergey, by Kelly Freas, by Alex Schomburg, by H. J. Ward, glossy pulp art, Amazing Stories, Weird Tales, 8k, high resolution, best quality",
                negative: input => $"{input.Negative}, text, cropped, bad art, deformed, worst quality, watermark, text"
            ),

            ["50s Infomercial Anime"] = new(
                prompt: input => $"{input.Description}, 1950s infomercial style, (delicate linework:1.1), paprika anime art style, close-up, (chromatic aberration glow:1.2), 2d painted cel animation, close-up, soft focus, 2D pixiv 1950s",
                negative: input => $"worst quality, low quality, {input.Negative}, neon tube, bad art, messy, disfigured, bad anatomy, out of focus, grainy, blurry, jpeg artifact noise, deformed"
            ),

            ["3D Pokemon"] = new(
                prompt: input => $"a pokemon creature, (({input.Description})), {(input.PokemonType != null ? input.PokemonType + ", " : "")}4k render, beautiful pokemon digital art, fakemon, pokemon creature, cryptid, fakemon, masterpiece, {Choose("soft", "sharp")}, (best quality, high quality:1.3)",
                negative: input => $"{input.Negative}, distorted, deformed, bad art, low quality, over saturated, extreme contrast, (worst quality, low quality:1.3)"
            ),

            ["Painted Pokemon"] = new(
                prompt: input => $"({input.Description}), {(input.PokemonType != null ? input.PokemonType + ", " : "")}4k digital painting of a pokemon, amazing pokemon creature art by piperdraws, cryptid creations by Piper Thibodeau, by Naoki Saito and {Choose("Tokiya", "Mitsuhiro Arita")}, incredible composition",
                negative: input => $"{input.Negative}, crappy 3D render, distorted, deformed, bad art, low quality, text, signature"
            ),

            ["2D Pokemon"] = new(
                prompt: input => $"({input.Description}:1.2), {(input.PokemonType != null ? input.PokemonType + ", " : "")}pokemon creature concept, superb line art, beautiful colors and composition, 2d art style, beautiful pokemon digital art, fakemon, by Sowsow, pokemon creature, cryptid, fakemon, masterpiece, by Yuu Nishida, 4k",
                negative: input => $"{input.Negative}, multiple, grid, (black and white), distorted, deformed, bad art, low quality, crappy 3D render, text watermark symbols logo"
            ),

            ["Vintage Anime"] = new(
                prompt: input => $"{input.Description}, 1990s anime, vintage anime, 90's anime style, by hajime sorayama, by greg tocchini, anime masterpiece, pixiv, akira-style art, akira anime art, 4k, high quality",
                negative: input => $"{input.Negative}, (worst quality, low quality:1.3), bad art, distorted, deformed"
            ),

            ["Neon Vintage Anime"] = new(
                prompt: input => $"{input.Description}, ((neon vintage anime)) style, 90's anime style, 1990s anime, hajime sorayama, greg tocchini, neon vintage anime masterpiece, anime art, 4k, high quality",
                negative: input => $"{input.Negative}, blurry, (worst quality, low quality:1.3), bad art, distorted, deformed"
            ),

            ["Manga"] = new(
                prompt: input => $"{input.Description}, incredible hand-drawn manga, black and white, by Takehiko Inoue, by Katsuhiro Otomo and akira toriyama manga, hand-drawn art by rumiko takahashi and Inio Asano, Ken Akamatsu manga art",
                negative: input => $"{input.Negative}, (worst quality, low quality:1.3), bad photo, bad 3D render, distorted, deformed, fuzzy, noisy, blurry, smudge"
            ),

            ["Fantasy World Map"] = new(
                prompt: input => $"beautiful fantasy map of {input.Description}, beautiful fantasy map inspired by middle earth and azeroth and discworld and westeros and essos and the witcher world and tamriel and faerûn and thedas, 4k, beautiful colors, crisp, high-resolution artistic map, topographic 3D terrain, artistic map",
                negative: input => $"{input.Negative}, low quality, blurry, worst quality, childrens drawing, boring, logo, scratchy and grainy, messy, washed out colors, sepia, hazy"
            ),

            ["Fantasy City Map"] = new(
                prompt: input => $"an aerial view of a city, TTRPG city map showing the full city, {input.Description}, fantasy art, by senior environment artist, beautiful fantasy map",
                negative: input => $"{input.Negative}, fuzzy, bad art"
            ),

            ["Old World Map"] = new(
                prompt: input => $"fantasy world map of {input.Description}, fantasy world map, highly detailed digital painting, fantasy art, map illustration, 8k",
                negative: input => $"{input.Negative}, low resolution, worst quality"
            ),

            ["3D Isometric Icon"] = new(
                prompt: input => $"{input.Description}, 3D isometric render of cute {input.Description}, 3D app icon, clean isometric design, beautiful design, soft gradient background, soft colors, centered, 3D blender render, masterpiece, best quality, high resolution, 8k octane render, beautiful color scheme, soft smooth lighting, physically based rendering, square image, high polycount",
                negative: input => $"{input.Negative}, low quality, terrible design, glitchy compression artefacts, deformed, blurry compression artefacts, text"
            ),

            ["Flat Style Icon"] = new(
                prompt: input => $"{input.Description}, creative icon, flat style icon, masterpiece, high resolution, crisp, beautiful composition and color choice, beautiful flat painted style, behance contest-winner, award winning icon illustration, 8k, best quality",
                negative: input => $"{input.Negative}, gradient, bad design, blurry, jpeg compression artefacts, grainy, gradient, text, messy and inconsistent"
            ),

            ["Flat Style Logo"] = new(
                prompt: input => $"beautiful flat-style logo design depicting {input.Description}, creative flat-style logo design, trending on dribbble, featured on behance, portfolio piece, minimal flat design, breathtaking graphic design, 8k, high resolution vector logo, plain background, amazingly beautiful logo design, winner of best logo award",
                negative: input => $"{input.Negative}, photo, hilariously bad design, bad composition, bad colors, blurry, worst quality, low quality, shadow, boring, bad dsign, worst design ever, hilariously bad design, drop-shadow, gradient, messy, chaotic"
            ),

            ["Game Art Icon"] = new(
                prompt: input => $"{input.Description}, a concept art icon for league of legends, a digital art logo, illustration, league of legends style icon, inspired by wlop style, 8k, dota 2 style icon, fine details, sharp, very detailed icon, high resolution rpg ability/spell/item icon",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn, multiple"
            ),

            ["Digital Painting Icon"] = new(
                prompt: input => $"{input.Description}, app logo icon, digital art pictogram icon, trending on artstation, app icon by atey ghailan, app icon by greg rutkowski, app icon by greg tocchini, app icon by james gilleard, 8k, high resolution, best quality",
                negative: input => $"{input.Negative}, photo, low-quality, deformed, text, poorly drawn"
            ),

            ["Concept Art Icon"] = new(
                prompt: input => $"{input.Description}, a concept art icon, a digital art logo, illustration, league of legends style concept art logo icon, inspired by wlop style, 8k, fine details, sharp, very detailed, high resolution logo icon",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn, multiple"
            ),

            ["Cute 3D Icon"] = new(
                prompt: input => $"{input.Description}, a cute 3D icon of {input.Description}, cartoon 3D icon, very cute shape, stylized octane render, 8k, masterpiece, soooo cute, beautiful cute perfection, beautiful soft lighting, soft colors, centered, high resolution, {input.Description}, soft gradient background",
                negative: input => $"{input.Negative}, low quality, text, ugly, gross"
            ),

            ["Cute 3D Icon Set"] = new(
                prompt: input => $"{input.Description}, a set of lovely little 3D icons, cute 3D icons, very cute shapes, stylized octane render, 8k, masterpiece, soooo cute, beautiful cute perfection, beautiful soft lighting, soft colors, centered, high resolution, {input.Description}, cute icon set",
                negative: input => $"{input.Negative}, low quality, text, ugly, gross"
            ),

            ["Crayon Drawing"] = new(
                prompt: input => $"{input.Description}, crayon drawing",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn"
            ),

            ["Pencil"] = new(
                prompt: input => $"((black and white pencil drawing)), {input.Description}, black and white, breathtaking pencil illustration, highly detailed, 4k, (textured paper), pencil texture, sketch",
                negative: input => $"{input.Negative}, stationary, holding pen, holding paper, low-quality, deformed, photo, 3D render, text, poorly drawn"
            ),

            ["Tattoo Design"] = new(
                prompt: input => $"amazing tattoo design, {input.Description}, breathtaking tattoo design, incredible tattoo design",
                negative: input => $"{input.Negative}, low-quality, poorly drawn, blurry, faded, terrible design, worst quality, deformed, amputee, ((words)), ((letters))"
            ),

            ["Watercolor"] = new(
                prompt: input => $"{input.Description}, watercolor painting",
                negative: input => $"{input.Negative}, low-quality, deformed, text, poorly drawn"
            ),
        };

        public string MakePrompt(Configuration input)
        {
            return prompt(input);
        }

        public string MakeNegative(Configuration input)
        {
            return negative(input);
        }
    }
}
