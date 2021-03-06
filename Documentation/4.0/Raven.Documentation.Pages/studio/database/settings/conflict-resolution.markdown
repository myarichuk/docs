﻿# Conflict Resolution
---

{NOTE: }

* Define the server behaviour upon a conflict between documents.  

* Conflicts that are created can be manually resolved in [Documents Conflicts View](../../../studio/database/documents/conflicts-view).  

* In this page:  
  * [The Conflicts Resolution View](../../../studio/database/settings/conflict-resolution#the-conflicts-resolution-view)  
  * [Script Examples](../../../studio/database/settings/conflict-resolution#script-examples)  
{NOTE/}

---

{PANEL: The Conflicts Resolution View}

![Figure 1. Conflicts Resolution View](images/conflict-resolution-view-1.png "Conflict Resolution")

1. **Save, Add, Modify or Delete** conflict resolution scripts.  

2. **Set default behaviour**  -  Conflicts will be created _only if_:  
   * This option is unchecked and no script is defined  
   * This option is unchecked and the script is defined but returns null  

3. **Resolution Script** (optional)  
   * Supply a javaScript function to resolve the conflicting documents.  
   
   * The object that is returned by the script will be used as the conflict resolution in the document.  
   
   * The sript is defined per collection  
   
   * Note: in case the script returns null (either intentionally, or upon some error),  
           the server will resolve the conflict according to the defined default behaviour  
           (i.e. resolve by using latest version or creating a conflict for the user to resolve).  
   
   * Script Variables:  
     * ***docs*** - the conflicted documents objects array  
     * ***hasTombstone*** - true if either of the conflicted documents was deleted  
     * ***resolveToTombstone*** - return this value from script if the resolution wanted is to delete this document  

{PANEL/}

{PANEL: Script Examples}

{NOTE: }

*  1. Resolve according to field content - return the highest value of the field  

{CODE-BLOCK:javascript}
// First conflicting document
{
    "Name": "John",
    "MaxRecord": 43
}
{CODE-BLOCK/}

{CODE-BLOCK:javascript}
// Second conflicting document
{
    "Name": "John",
    "MaxRecord": 80
}
{CODE-BLOCK/}

{CODE-BLOCK:javascript}
// The resolving script:
var maxRecord = 0;
for (var i = 0; i < docs.length; i++) {
    maxRecord = Math.max(docs[i].maxRecord, maxRecord);
}
docs[0].MaxRecord = maxRecord;

return docs[0];
{CODE-BLOCK/}
{NOTE/}

{NOTE: }

* 2. Resolve by deleting the document  

{CODE-BLOCK:javascript}
if (hasTombstone) {
    return resolveToTombstone;
}
{CODE-BLOCK/}
{NOTE/}

{NOTE: }

* 3. The metadata can also be accessed - return the document that has the largest number of attachments  

{CODE-BLOCK:javascript}
var result = docs[0];

for (var i = 1; i < docs.length; i++) {
     if (docs[i]["@metadata"]["@attachments"].length > result["@metadata"]["@attachments"].length)
        result = docs[i];
    }
}

return result;
{CODE-BLOCK/}
{NOTE/}
{PANEL/}

## Related Articles

- [Documents Conflicts View](../../../studio/database/documents/conflicts-view)  
- [What is a Conflict](../../../server/clustering/replication-conflicts#what-is-a-conflict)  
