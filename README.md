# Intelligent-Bot
A bot which uses WordNet to parse and match results from a given source to answer questions. Also weighs context in reference to tags in structured documents for instance bold tags, headers, etc.
-Added support for any generic web page
-Pdf support added
<a href="https://teams.microsoft.com/l/chat/0/0?users=28:d54d718d-8ebf-4fa4-b34a-c445cadd7bbb">Microsoft Teams link for the bot</a>
[![N|Solid](https://dev.botframework.com/client/images/channels/icons/msteams.png)](https://teams.microsoft.com/l/chat/0/0?users=28:d54d718d-8ebf-4fa4-b34a-c445cadd7bbb) Click to add to Microsoft Teams
Please drop me a mail in case channel is down, it's hosted on my free Azure subscription which is limited :)

### Working scenarios


### Known issues
- Currently based on the WordNet nouns, so accuracy for different cases can vary.
- PDF parsing fails for certain files

### Todos
- Accuracy for results can easily be improved by utilising WordNet verbs as well along with POS tagging for query.
- Find a more reliable way of parsing PDF