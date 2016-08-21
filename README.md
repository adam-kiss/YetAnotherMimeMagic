YetAnotherMimeMagic is a library to detect the mime type of a file by content or by extension. This implementation was inspired 
by Daniel Mendler's MimeMagic project wiritten in Ruby. (https://github.com/minad/mimemagic)

This project also uses the mime database provided by freedesktop.org (see http://freedesktop.org/wiki/Software/shared-mime-info/).

Usage
=====

    var fileInfo = new FileInfo("-- path --");
    
    var result = fileInfo.FindMimeType();
    
    Console.WriteLine($" filename:           { fileInfo.FullName }");
    Console.WriteLine($" mime-type:          { result.MimeType }");
    Console.WriteLine($" acronym:            { results.Acronym }");
    Console.WriteLine($" found by content:   { results.FoundByContent }");
    Console.WriteLine($" found by extension: { results.FoundByExtension }");
    Console.WriteLine($" elasped time in ms: { results.ElapsedtimeInMS }");

Authors
=======

Adam Kiss
