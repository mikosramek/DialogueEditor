# Created by Miko Sramek
Contact: contact@mikosramek.ca
@MikoGGSramek
14/12/18           

//////////////////////////////////

# Current Features:

Create NPCProfiles that can contain different moods and sprites.
Create Dialogue nodes that use the profiles, have text, and can link to other nodes via responses.



# Planned Features:

Events (but probably not, because it's too specific!)


# Bugs / Weirdness:

NPCProfile assets need to end with "_Profile" for the editor to find them
Right clicking a node -> Reset Talking Point will reset it to default size (ie, it has no responses) so it'll get weird...


# Usage:

The DialogueHolder asset has a list of Talking Point objects.


Each talking point has a list of responses. These reponses have text, and an id. That id refers to the next talking point. Using DialogueHolder's FindViaID(int id), you can find the next
talking point object.

How you handle that access / callback is up to you.

A response will start with the ID of -1.

Character Portraits from:
https://opengameart.org/content/flare-portrait-pack-number-one
