using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmojiOne.Tests {
    [TestClass]
    public class EmojiOneTests {

        [TestMethod]
        public void AsciiToUnicode() {
            // single smiley
            string text = ":D";
            string expected = "😃";
            string actual = EmojiOne.AsciiToUnicode(text);
            Assert.AreEqual(expected, actual);

            // single smiley with incorrect case (shouldn't convert)
            text = ":d";
            expected = text;
            actual = EmojiOne.AsciiToUnicode(text);
            Assert.AreEqual(expected, actual);

            // multiple smileys
            text = ";) :P :* :)";
            expected = "😉 😛 😘 🙂";
            actual = EmojiOne.AsciiToUnicode(text);
            Assert.AreEqual(expected, actual);

            // smiley to start a sentence
            text = @":\ is our confused smiley.";
            expected = "😕 is our confused smiley.";
            actual = EmojiOne.AsciiToUnicode(text);
            Assert.AreEqual(expected, actual);

            // smiley to end a sentence
            text = "Our smiley to represent joy is :')";
            expected = "Our smiley to represent joy is 😂";
            actual = EmojiOne.AsciiToUnicode(text);
            Assert.AreEqual(expected, actual);

            // smiley to end a sentence with puncuation
            text = "The reverse to the joy smiley is the cry smiley :'(.";
            expected = "The reverse to the joy smiley is the cry smiley 😢.";
            actual = EmojiOne.AsciiToUnicode(text);
            Assert.AreEqual(expected, actual);

            // smiley to end a sentence with preceeding punctuation
            text = @"This is the ""flushed"" smiley: :$.";
            expected = @"This is the ""flushed"" smiley: 😳.";
            actual = EmojiOne.AsciiToUnicode(text);
            Assert.AreEqual(expected, actual);

            // smiley inside of an IMG tag (shouldn't convert anything inside of the tag)
            text = $@"Smile <img class=""emojione"" alt="":)"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f604.png"" /> because it's going to be a good day.";
            expected = text;
            actual = EmojiOne.AsciiToUnicode(text);
            Assert.AreEqual(expected, actual);

            // typical username password fail  (shouldn't convert the user:pass, but should convert the last :P)
            text = @"Please log-in with user:pass as your credentials :P.";
            expected = @"Please log-in with user:pass as your credentials 😛.";
            actual = EmojiOne.AsciiToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shouldn't replace an ascii smiley in a URL (shouldn't replace :/)
            text = @"Check out http://www.emojione.com";
            expected = text;
            actual = EmojiOne.AsciiToUnicode(text);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void ShortnameToImage() {
            // shortname to image
            string text = "Hello world! 😄 :smile:";
            string expected = $@"Hello world! 😄 <img class=""emojione"" alt=""😄"" title="":smile:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f604.png"" />";
            string actual = EmojiOne.ShortnameToImage(text);
            Assert.AreEqual(expected, actual);

            // shortname at start of sentence with apostrophe
            text = ":snail:'s are cool!";
            expected = $@"<img class=""emojione"" alt=""🐌"" title="":snail:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f40c.png"" />'s are cool!";
            actual = EmojiOne.ShortnameToImage(text);
            Assert.AreEqual(expected, actual);

            // shortname shares a colon
            text = ":invalid:snail:";
            expected = $@":invalid<img class=""emojione"" alt=""🐌"" title="":snail:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f40c.png"" />";
            actual = EmojiOne.ShortnameToImage(text);
            Assert.AreEqual(expected, actual);

            // mixed ascii, regular unicode and duplicate emoji
            text = ":alien: is 👽 and 저 is not :alien: or :alien: also :randomy: is not emoji";
            expected = $@"<img class=""emojione"" alt=""👽"" title="":alien:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f47d.png"" /> is 👽 and 저 is not <img class=""emojione"" alt=""👽"" title="":alien:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f47d.png"" /> or <img class=""emojione"" alt=""👽"" title="":alien:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f47d.png"" /> also :randomy: is not emoji";
            actual = EmojiOne.ShortnameToImage(text);
            Assert.AreEqual(expected, actual);

            // multiline emoji string
            text = ":dancer:\n:dancer:";
            expected = $"<img class=\"emojione\" alt=\"💃\" title=\":dancer:\" src=\"{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f483.png\" />\n<img class=\"emojione\" alt=\"💃\" title=\":dancer:\" src=\"{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f483.png\" />";
            actual = EmojiOne.ShortnameToImage(text);
            Assert.AreEqual(expected, actual);

            // triple emoji string
            text = ":dancer::dancer::alien:";
            expected = $@"<img class=""emojione"" alt=""💃"" title="":dancer:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f483.png"" /><img class=""emojione"" alt=""💃"" title="":dancer:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f483.png"" /><img class=""emojione"" alt=""👽"" title="":alien:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f47d.png"" />";
            actual = EmojiOne.ShortnameToImage(text);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShortnameToUnicode() {
            // shortname to unicode
            string text = "Hello world! 😄 :smile:";
            string expected = "Hello world! 😄 😄";
            string actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // single shortname
            text = ":snail:";
            expected = "🐌";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shortname mid sentence with a comma
            text = "The :unicorn:, is Emoji One's official mascot.";
            expected = "The 🦄, is Emoji One's official mascot.";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shortname at start of sentence
            text = ":snail: mail.";
            expected = "🐌 mail.";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shortname at start of sentence with apostrophe
            text = ":snail:'s are cool!";
            expected = "🐌's are cool!";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shortname shares a colon
            text = ":invalid:snail:";
            expected = ":invalid🐌";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // mixed ascii, regular unicode and duplicate emoji
            text = ":alien: is 👽 and 저 is not :alien: or :alien: also :randomy: is not emoji";
            expected = "👽 is 👽 and 저 is not 👽 or 👽 also :randomy: is not emoji";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // multiline emoji string
            text = ":dancer:\n:dancer:";
            expected = "💃\n💃";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // triple emoji string
            text = ":dancer::dancer::alien:";
            expected = "💃💃👽";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shortname at end of sentence
            text = "Emoji One's official mascot is :unicorn:.";
            expected = "Emoji One's official mascot is 🦄.";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shortname at end of sentence with alternate punctuation
            text = "Emoji One's official mascot is :unicorn:!";
            expected = "Emoji One's official mascot is 🦄!";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shortname at end of sentence with preceeding colon
            text = "Emoji One's official mascot: :unicorn:";
            expected = "Emoji One's official mascot: 🦄";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shortname inside of IMG tag
            text = $@"The <img class=""emojione"" alt="":unicorn:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f984.png"" /> is Emoji One's official mascot.";
            expected = text;
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shortname to unicode with code pairs
            text = ":nine:";
            expected = "9⃣";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            // shortname alias
            text = ":poo:";
            expected = "💩";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToShort() {
            // to short
            string text = "Hello world! 😄 :smile:";
            string expected = "Hello world! :smile: :smile:";
            string actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // single unicode character conversion
            text = "🐌";
            expected = ":snail:";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // mixed ascii, regular unicode and duplicate emoji
            text = "👽 is not :alien: and 저 is not 👽 or 👽";
            expected = ":alien: is not :alien: and 저 is not :alien: or :alien:";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // multiline emoji string
            text = "💃\n💃";
            expected = ":dancer:\n:dancer:";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // single character with surrogate pair
            text = "9⃣";
            expected = ":nine:";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // character mid sentence
            text = "The 🦄 is Emoji One's official mascot.";
            expected = "The :unicorn: is Emoji One's official mascot.";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // character mid sentence with a comma
            text = "The 🦄, is Emoji One's official mascot.";
            expected = "The :unicorn:, is Emoji One's official mascot.";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // character at start of sentence
            text = "🐌 mail.";
            expected = ":snail: mail.";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // character at start of sentence with apostrophe
            text = "🐌's are cool!";
            expected = ":snail:'s are cool!";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // character at end of sentence
            text = "Emoji One's official mascot is 🦄.";
            expected = "Emoji One's official mascot is :unicorn:.";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // character at end of sentence with alternate puncuation
            text = "Emoji One's official mascot is 🦄!";
            expected = "Emoji One's official mascot is :unicorn:!";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // character at end of sentence with preceeding colon
            text = "Emoji One's official mascot: 🦄";
            expected = "Emoji One's official mascot: :unicorn:";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // character inside of IMG tag
            text = $@"The <img class=""emojione"" alt=""🦄"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f984.png"" /> is Emoji One's official mascot.";
            expected = text;
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            // unicode alternate to short
            text = "#️⃣"; // 0023-fe0f-20e3
            expected = ":hash:";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UnifyUnicode() {
            // unify unicode
            string text = "Hello world! 😄 :smile:";
            string expected = "Hello world! 😄 😄";
            string actual = EmojiOne.UnifyUnicode(text);
            Assert.AreEqual(expected, actual);

            // mixed ascii, regular unicode and duplicate emoji
            text = ":alien: is 👽 and 저 is not :alien: or :alien: also :randomy: is not emoji";
            expected = "👽 is 👽 and 저 is not 👽 or 👽 also :randomy: is not emoji";
            actual = EmojiOne.UnifyUnicode(text);
            Assert.AreEqual(expected, actual);

            // multiline emoji string
            text = ":dancer:\n:dancer:";
            expected = "💃\n💃";
            actual = EmojiOne.UnifyUnicode(text);
            Assert.AreEqual(expected, actual);

            // triple emoji string
            text = ":dancer::dancer::alien:";
            expected = "💃💃👽";
            actual = EmojiOne.UnifyUnicode(text);
            Assert.AreEqual(expected, actual);

            // single unicode character conversion
            text = ":snail:";
            expected = "🐌";
            actual = EmojiOne.UnifyUnicode(text);
            Assert.AreEqual(expected, actual);

            // mixed unicode, shortname and ascii conversion
            text = "😄 :smile: :)";
            expected = "😄 😄 🙂";
            actual = EmojiOne.UnifyUnicode(text, ascii: true);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToImage() {
            // to image
            string text = "Hello world! 😄 :smile:";
            string expected = $@"Hello world! <img class=""emojione"" alt=""😄"" title="":smile:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f604.png"" /> <img class=""emojione"" alt=""😄"" title="":smile:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f604.png"" />";
            string actual = EmojiOne.ToImage(text);
            Assert.AreEqual(expected, actual);

            // mixed ascii, regular unicode and duplicate emoji
            text = ":alien: is 👽 and 저 is not :alien: or :alien: also :randomy: is not emoji";
            expected = $@"<img class=""emojione"" alt=""👽"" title="":alien:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f47d.png"" /> is <img class=""emojione"" alt=""👽"" title="":alien:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f47d.png"" /> and 저 is not <img class=""emojione"" alt=""👽"" title="":alien:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f47d.png"" /> or <img class=""emojione"" alt=""👽"" title="":alien:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f47d.png"" /> also :randomy: is not emoji";
            actual = EmojiOne.ToImage(text);
            Assert.AreEqual(expected, actual);

            // single shortname conversion
            text = ":snail:";
            expected = $@"<img class=""emojione"" alt=""🐌"" title="":snail:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f40c.png"" />";
            actual = EmojiOne.ToImage(text);
            Assert.AreEqual(expected, actual);

            // shortname shares a colon
            text = ":invalid:snail:";
            expected = $@":invalid<img class=""emojione"" alt=""🐌"" title="":snail:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f40c.png"" />";
            actual = EmojiOne.ToImage(text);
            Assert.AreEqual(expected, actual);

            // single unicode character conversion
            text = "🐌";
            expected = $@"<img class=""emojione"" alt=""🐌"" title="":snail:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f40c.png"" />";
            actual = EmojiOne.ToImage(text);
            Assert.AreEqual(expected, actual);

            // mixed unicode, shortname and ascii conversion
            text = "😄 :smile: :D";
            expected = $@"<img class=""emojione"" alt=""😄"" title="":smile:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f604.png"" /> <img class=""emojione"" alt=""😄"" title="":smile:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f604.png"" /> <img class=""emojione"" alt=""😃"" title="":smiley:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f603.png"" />";
            actual = EmojiOne.ToImage(text, ascii: true);
            Assert.AreEqual(expected, actual);

            // shortname alt
            text = "😄";
            expected = $@"<img class=""emojione"" alt="":smile:"" title="":smile:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f604.png"" />";
            actual = EmojiOne.ToImage(text, unicodeAlt: false);
            Assert.AreEqual(expected, actual);

            // sprite
            text = "😄";
            expected = @"<span class=""emojione emojione-32-people _1f604"" title="":smile:"">😄</span>";
            actual = EmojiOne.ToImage(text, sprite: true);
            Assert.AreEqual(expected, actual);

            // sprite nature
            text = "🐱";
            expected = @"<span class=""emojione emojione-32-nature _1f431"" title="":cat:"">🐱</span>";
            actual = EmojiOne.ToImage(text, sprite: true);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShortnameToAscii() {
            string text = ":smiley: :slight_smile:";
            string expected = @":D :)";
            string actual = EmojiOne.ShortnameToAscii(text);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UnicodeToCodepoint() {
            string unicode = "😀"; // :grinning:
            string expected = "1f600";
            string actual = EmojiOne.ToCodePoint(unicode);
            Assert.AreEqual(expected, actual);

            expected = "D83D-DE00";
            actual = ShowX4(unicode);
            Assert.AreEqual(expected, actual);

            string codepoint = "1f600";
            expected = "😀";
            actual = EmojiOne.ToUnicode(codepoint);
            Assert.AreEqual(expected, actual);
            expected = "D83D-DE00";
            actual = ShowX4(actual);
            Assert.AreEqual(expected, actual);

            expected = "\uD83D\uDE00";
            actual = "😀";
            Assert.AreEqual(expected, actual);

            expected = "\\uD83D\\uDE00";
            actual = ToSurrogateString("1f600");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NumberShouldNotBeConvertedToShortname() {
            string text = "2";
            string notexpected = ":digit_two:";
            string actual = EmojiOne.ToShort(text);
            Assert.AreNotEqual(notexpected, actual);
        }

        [TestMethod]
        public void Diversity() {
            string text = "👍";
            string expected = ":thumbsup:";
            string actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            text = "👍🏻";
            expected = ":thumbsup_tone1:";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            expected = $@"<img class=""emojione"" alt=""👍🏻"" title="":thumbsup_tone1:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f44d-1f3fb.png"" />";
            actual = EmojiOne.ToImage(text);
            Assert.AreEqual(expected, actual);

            expected = @"<span class=""emojione emojione-32-diversity _1f44d-1f3fb"" title="":thumbsup_tone1:"">👍🏻</span>";
            actual = EmojiOne.ToImage(text, sprite: true);
            Assert.AreEqual(expected, actual);

            text = "👍🏿";
            expected = ":thumbsup_tone5:";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            expected = $@"<img class=""emojione"" alt=""👍🏿"" title="":thumbsup_tone5:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f44d-1f3ff.png"" />";
            actual = EmojiOne.ToImage(text);
            Assert.AreEqual(expected, actual);

            expected = @"<span class=""emojione emojione-32-diversity _1f44d-1f3ff"" title="":thumbsup_tone5:"">👍🏿</span>";
            actual = EmojiOne.ToImage(text, sprite: true);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FamilyEmoji() {
            string unicode = "👨‍👩‍👧‍👦";
            string codepoint = EmojiOne.ToCodePoint(unicode);
            string expected = "1f468-200d-1f469-200d-1f467-200d-1f466";
            Assert.AreEqual(expected, codepoint);

            unicode = EmojiOne.ToUnicode(codepoint);
            string shortname = EmojiOne.ToShort(unicode);
            expected = ":family_mwgb:";
            Assert.AreEqual(expected, shortname);

            unicode = "👨‍👩‍👧‍👦";
            shortname = EmojiOne.ToShort(unicode);
            expected = ":family_mwgb:";
            Assert.AreEqual(expected, shortname, "Going from unicode > codepoint > unicode > shortname works, but going directly from unicode > shortname fails");
        }

        [TestMethod]
        public void Version224Emoji() {
            // test that new emoji from v2.2.4 works
            string text = ":first_place:";
            string expected = $@"<img class=""emojione"" alt=""🥇"" title="":first_place:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f947.png"" />";
            string actual = EmojiOne.ShortnameToImage(text);
            Assert.AreEqual(expected, actual);

            text = ":avocado:";
            expected = "🥑";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);

            text = "🖤";
            expected = ":black_heart:";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Version40Emoji() {
            // test that new emoji from v4.0 works
            string text = ":cold_face:";
            string expected = $@"<img class=""emojione"" alt=""🥶"" title="":cold_face:"" src=""{EmojiOne.ImagePath}{EmojiOne.EmojiSize}/1f976.png"" />";
            string actual = EmojiOne.ShortnameToImage(text);
            Assert.AreEqual(expected, actual);

            text = "🧁";
            expected = ":cupcake:";
            actual = EmojiOne.ToShort(text);
            Assert.AreEqual(expected, actual);

            text = ":superhero:";
            expected = "🦸";
            actual = EmojiOne.ShortnameToUnicode(text);
            Assert.AreEqual(expected, actual);
        }

        private static string ShowX4(string s, int? i = null) {
            string s2 = "";
            for (int x = 0; x < s.Length; x++) {
                s2 += string.Format("{0:X4}{1}", (int)s[x], ((x == s.Length - 1) ? String.Empty : "-"));
            }
            if (i != null) {
                return string.Format("{0} => {1:X}", s2, i);
            } else {
                return s2;
            }
        }

        /// <summary>
        /// Converts a unicode character to surrogate pairs
        /// </summary>
        /// <param name="unicode"></param>
        /// <returns></returns>
        private static string ToSurrogateString(string codepoint) {
            string unicode = EmojiOne.ToUnicode(codepoint);
            string s2 = "";
            for (int x = 0; x < unicode.Length; x++) {
                s2 += string.Format("\\u{0:X4}", (int)unicode[x]);
            }
            return s2;
        }

    }
}
