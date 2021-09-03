# Shattered Skies

"The sky, frozen over, crashing down to the earth. Isn't it lovely?" 

Use a huge greatsword to slash your way through a deserted riverside village, in pursuit of a mysterious masked man. 

This was my submission to "[Brackey's Game Jam 2021.2](https://itch.io/jam/brackeys-6)", a 7 day game jam.

You can download the game for free [here, on itch,.io](https://request.itch.io/shattered-skies), 
or you can check out my other projects at [request.moe](https://request.moe).

You're free to do whatever with this code, but if you do use it, it'd be real cool of you to link back to this page or the itch.io page (or both). Thanks!


## Setup

  1. Clone this repo
  2. Install the [SteamVR Plugin](https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647), as well as [Hurricane VR](https://assetstore.unity.com/packages/tools/physics/hurricane-vr-physics-interaction-toolkit-177300)


## Some topics of interest in this repo

### Swords with weight, and stamina in VR

In this game, you wield a very large sword to fight enemies. You also have a limited stamina pool, which depletes and regenerates quickly.
The more you move your sword around, the faster your stamina depletes. The lower your stamina, the harder it is to move your sword around.

This resource management makes for an interesting game loop - do I sprint up and attack the lizard enemy while its back is turned, or do I wait for it to come closer so I can attack with more stamina?
It's much harder to block projectiles when you can't lift your sword properly, so how much should the player prioritize defense over raw offense?

The way that this was implemented was by adjusting the mass of the rigidbody each frame depending on the percentage of stamina the player had available. 
Stamina loss from the sword was calculated based on per-frame velocity, measured at the tip of the sword. 

The weight range of the sword was configured such that the player could wield the sword single-handedly at full stamina, but could barely pick it up at low stamina.
An unexpected bonus of tweaking the mass of the rigidbody (rather than tweaking the strength of the hand's config joint) was that it prevented the player from abusing the strength of the physics hand, and simply lifting the sword without actually grabbing it.

