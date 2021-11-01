# Outlook Email Parser and Segmenter

This is a small library, that performs naive processing of the emails stored in Microsoft Office Outlook, to segment out, lazily, messages parts, like there header, body, signature, and replays.

You can use it to segment emails not only from Outlook. I created this to apply afterwards, NLP on the results.

## Features

- Segments each email into 3 parts: Header, Email Body, Signature.
- Segmenting out the replays that are included in the email, and segments each replay to above mentioned three parts.
- Process replay headers, to extract information like sender, date, subject, phones etc...
- Performs special HTML tag cleaning, to remove styles and other visual noise, but to keep the structure. This saves a lot of space if you store the segmentation results in a database.
- Extracts pure text, cleans it from bad characters, and converts all characters to readable ones, such that original email visual structure is preserved.
- Detecting and excluding repeated results, so similar signatures or replays, will not consume memory or space (done by calculating hashes).
- Some support for mail clients of different languages.
- Parallelizing segmentation procedure, for better performance with huge amounts of emails.

## Why Processing Replays?

Processing replays may be needed for various reasons. Particularly, Outlook conversations (that returned by Outlook itself), are not always enough, for example, in case when we want to understand the whole message context.

For Instance, there is no guarantee that all of the replays in the given message, are also available in your Outlook inbox, through the conversations. This happens for example when a totally new message had been forwarded to you with all of the replays.

## Usage

The below is pretty straightforward:

```csharp
// establish a connection to Outlook
Outlook.Connect();

// get 100 emails from first folder in first sote (i.e.
// account or pst file),and select 10th email
var email = Outlook.Stores[0].Folders[0].FetchLatest(0, 99)[10];

// get email info;
var to = email.Header.To;
var text = email.Body.HTML;

// get 3'rd replay info
var replay = email.Replays[3];
var replay_subject = replay.Header.Subject;
var sign_emails = replay.Signature.Body.EmailAddress;

// if Outlook.CheckForIdenticalChunks = true, the body property can be
// null, since it may be a copy of another one, then access its info from:
var sign_html = replay.Signature.Body.BaseBodySegment.HTML;
```

There is also a bunch of options that can be tuned as per your needs, and affects the parsing performance:

```csharp
// Save space and memory, by comparing extracted text from
// different email parts, and saving a reference to the same text,
// instead of saving text itself. For example, when you have
// many repeating signatures in different mails.
Outlook.CheckForIdenticalChunks = True;

// If you have a multi-core CPU, this will significantly
// increase processing time, also it assumes that you want
// to pre-load all properties, that are lazy by default
Outlook.ProcessInParallel = True;

// other options, like greedy headers, signatures, replays processing (during parallel fetching) are available.
```

## Accuracy

**Do not expect to get perfect segmentation results.** Email segmentation is actually a very error-prone procedure, the reason is, that there are no standardized methods among different email clients (and even among different versions of the same client) to mark the replay, header, or signature sections, see for example [this answer](https://stackoverflow.com/a/279417).

However, if most of the emails that you are processing, are actually B2B correspondence (i.e. from and to business/corporate), then you can expect very good results since they usually use MS Outlook as an email client, and it does mark the above-mentioned segments in a particular way, that I exploit in this library. However, there are still no guarantees.

For example, Outlook usually does wraps signatures with a div tag, however, sometimes, users manage to expand this div tag, by writing text inside the signature itself, without visually noticing this. In such cases, there is no way to distinguish the signature from the mail body by using my simple parsing approach.

If segmentation accuracy is critical in your task, you will need to use more sophisticated approaches, particularly machine learning, for example, see [this paper](https://www.cs.cmu.edu/~wcohen/postscript/email-2004.pdf), and this [project](https://github.com/HPI-Information-Systems/QuaggaLib). However, you still can use this library to strip out the main mail text part, to process it further by ML. Moreover, there are even specialized companies that advertise WebAPI for email segmentation.

## Performance

In the current implementation, with all options on, it can process 1000 messages (including all replays, that where in average 8 replays per message) in about two minutes, by using 6 core 9th Gen. Intel processor.

However, there are places for more optimization. And of course, turning some options off may lead to significantly less processing time.

## Dependencies

It build with .Net Framework 5, and :

> Microsoft.Office.Interop.Outlook.dll, v15.0.4569.1507

usually installed along with MS Office, and can be found in GAC. If you facing problems with it and .Net, check out [this answer](https://stackoverflow.com/questions/58130446/net-core-3-0-and-ms-office-interop). Also:

> HtmlAgilityPack, Nuget Package, v1.11.37

To use XPath 2.0 features:

> XPath2, Nuget Package, v1.1.2
