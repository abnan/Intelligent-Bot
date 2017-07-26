# Intelligent-Bot
A bot which uses WordNet to parse and match results from a given source to answer questions. Also weighs context in reference to tags in structured documents for instance bold tags, headers, etc. Also retains context of question subject so you continue asking questions on the same topic.
<a href="https://teams.microsoft.com/l/chat/0/0?users=28:d54d718d-8ebf-4fa4-b34a-c445cadd7bbb">Microsoft Teams link for the bot</a>
Please drop me a mail in case channel is down, it's hosted on my free Azure subscription which has limited usage :)

### Working scenarios
> Note: Relevant answers are highlighted for documentation purposes. They won't appear in Teams; Top 5 results are shown.

For querying Wikipedia, use this template to set the context "tell me about XYZ". Continue asking questions once context is set.
![Querying Wikipedia](../master/Data/1.png)
To query a webpage, mention url as "source: https://www.microsoft.com/en-us/software-download/faq" Works particularly well with FAQ, Q&A pages.
![Querying webpage](../master/Data/2.png)
To query a hosted pdf, provide url as "source: www.textbooksonline.tn.nic.in/books/11/std11-biozoo-em.pdf" Note: Some pdf versions are known to cause issues
![Querying pdf](../master/Data/3.png)

### Known issues
- Currently based on the WordNet nouns, so accuracy for different cases can vary.
- PDF parsing fails for certain files
- Wikipedia querying fails if multiple pages exist for same query eg. "EFL"

### Todos
- Accuracy for results can easily be improved by utilising WordNet verbs as well along with POS tagging for query.
- Find a more reliable way of parsing PDF
- Add support for ambiguous Wikipedia searches