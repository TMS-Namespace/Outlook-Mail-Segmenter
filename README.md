# Outlook Email Parser and Segmenter

This is a small library, that doing a naive processing of the emails stored in Microsoft Office Outlook, to segment out messages parts, like its header, body, signature and replays.

I created this to apply further NLP processing on emails.

# Features

-  Segments each email into 3 parts: Header, Email Body, Signature. 
- Segmenting out the replays that are included in the email, and segments each replay to above mentioned three prats.
- Process replay headers, to extract information like sender, date and subject etc..
- Performs special HTML tag cleaning, to remove styles and other visual noise, but to keeps structure. This saves a lot of space, if you store the segmentation results in a database.
- Extracts text, cleans it from bad characters and converts all characters to readable one, such that original email visual structure is preserved.
- Some support for mail clients of different languages.
- Paralyzing segmentation procedure, for better performance with large amounts of emails.

# Why Processing Replays?

Processing replays, may be needed for various reasons. Particularly, Outlook conversations (that returned by Outlook itself), are not enough, for example, to understand the whole context. 

For Instance, there is no guarantee that all of the replays that in the given message, are also available in your Outlook inbox. This happens for example, when a totally new message had been forwarded to you with all of the replays.

# Usage

The below is pretty straight forward:

```csharp
// establish a connection to Outlook
Outlook.Connect();

// define from where emails should be fetched.
// choose an account or an attached to Outlook .pst 
// file (all called EmailSources), and a folder
var folder = Outlook.EmailStores[0]?.Folders[0]?
// fetch and process first 100 emails
folder.Emails.Fetch(0,99);

// lets get some email data
// get 10th email
var email = emails[10]
// get sender's email address;
var toEmailAddress = email.Header.To;
// get email's cleaned HTML
// any segmented part of the email, including its body, is 
// represented but EmailChunk object, that has HTML as well 
// as Text properties
var text =  email.Body.HTML;

// get 3'rd replay
var thirdReplay = email.Replays[3];
// get replay signature text, if any found, by checking its 
// body, which is EmailChunk object
var replaySignatureText = thirdReplay.Signature?.Body?.Text;
// the body can be null, if current signature is been 
// found in another email, and :
// Outlook.CheckForIdenticalChunks = True, then we reference 
// base email chunk for information as follows
var replaySignatureText = thirdReplay.Signature?.Body?.BaseChunk.Body.Text;
// parse and get all emails in the signature
var replaySignatureEmails = thirdReplay.Signature?.Body.EmailAddresses;
```

There is also a bunch of options that can be tuned as per your needs, and affects the parsing performance:

```csharp
// Analyse and process all replays of all messages.
// if off, the analysis is done only to separate the
// Main message form the rest of replays
Outlook.ProcessAllReplays = True;

// Process mail and replay headers, and extract email address.
// set off if only message body is of your concern.
Outlook.ProcessHeaders = True;

// Process all signatures, that is, detect them, remove them
// from mail body, and keep a copy of their html and text
Outlook.ProcessSignatures = True;

// Save space and memory, by comparing extracted text from
// different email parts, and saving a reference to same text,
// instead of saving text it self. For example, when you have
// many repeating signatures in different mails.
Outlook.CheckForIdenticalChunks = True;

// If you have multi-core CPU, this will significantly
// increase processing time
Outlook.ProcessInParallel = True;
```

# Accuracy

**Do not expect to get perfect segmentation results.** Email segmentation is actually very error prone procedure, the reason is, that there is no standardized methods among different email clients (and even among different versions of the same client) to mark the replay, header, or signature sections, see for example [Duck Duck Go](https://duckduckgo.com).

However, if most of the emails that you are processing, are actually B2B correspondence (i.e. from and to bushiness/corporate), then you can expect a very good results, since they usually using MS Outlook as an email client, and it dose mark above mentioned segments in a particular way, that I exploit in this library. However, there is still no guaranties.

For example, Outlook usually do wraps signatures with div tag, however, sometimes, users manages to expand this div tag, by writing text inside the signature itself, without visually noticing this. In such cases, there is no way to distinguish the signature from the mail body by using my simple parsing approach.

If segmentation accuracy is critical in your task, you will need to use more sophisticated approaches, particularly machine learning, for example see this link. However, you still can use this library to strip out main mail text part, to process it farther by ML. Moreover, there is even a specialized companies that advertise WebAPI for email segmentation.

# Performance

In current implementation, with all options on, it can process 1000 messages (including all replays, that can be 10-30 messages) in about 2 minutes, by using 6 core 9th Gen. Intel processor.

However, there are places for more optimization. And off course, turning some options off, may lead to a significantly less processing time.

# Dependencies

> Microsoft.Office.Interop.Outlook.dll, usually installed along with MS Office, and can be found in GAC.

> HTMLAgility Nuget Package.
