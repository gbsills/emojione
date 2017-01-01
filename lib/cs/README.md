# EmojiOne utility for C# 

A C# implementation of [EmojiOne](https://github.com/Ranks/emojione) for converting Unicode emoji characters to :shortnames: and :shortnames: to emoji images etc. 

## Installation

To install EmojiOne, run the following command in the [Nuget Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console)

    PM> Install-Package EmojiOne

## Usage

After installation, use the following methods to convert Unicode emoji characters to :shortnames: and :shortnames: to emoji images etc.

`EmojiOne.ToShort()`  
Converts unicode emoji to shortnames.

`EmojiOne.ShortnameToImage()`  
Takes input containing emoji shortnames and converts it to emoji images.

`EmojiOne.UnicodeToImage()`  
Takes native unicode emoji input, such as that from your mobile device, and outputs image markup (png or svg).

`EmojiOne.ToImage()`  
Takes an input string containing both native unicode emoji and shortnames, and translates it into emoji images for display.

---

`EmojiOne.ShortnameToAscii()`  
Replaces shortnames with their ascii equivalent. This is useful for systems that don't support unicode or images.

`EmojiOne.ShortnameToUnicode()`  
Converts shortname emoji to unicode, useful for sending emoji back to mobile devices.

`EmojiOne.UnifyUnicode()`  
Unifies all emoji to their standard unicode types. 

## Building from source

* Running `build.cmd` will compile and package the project into a nuget package.
