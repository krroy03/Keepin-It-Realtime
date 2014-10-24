Project proposal feedback
==================

**Team number**: 94<br>
**Team name**: Keepin' It Realtime<br>
**Team members**: akshalab, alanchan, mechen, rkoganti

### Arthur
Good project proposal and the components seem like good building blocks. I'm not sure which part of the backend requires a distributed system though, as it seems like most of the server-side can be taken care of with a single server. Socket.io is a solid choice for realtime communications. Note that Bitnami does not provide hosting, but only the management of EC2 instances. You have to pay (or consume AWS credits) separately for the EC2 instances. Remember, this is a web applications class and at the end of the day we want you to build a webapp. Even though the core part of the game will be in Unity (how about HTML5 canvas?) you need to make sure you have a decent amount of front-end web implementations (looks like this is possible with the messaging system)

### Bailey
This seems like this is a project which could have a lot of interesting technical problems. Keep in mind that this isn't a game design class, so I think that most efforts should be devoted to writing high quality web application components. A very good game that doesn't demonstrate many of the course's concepts will most likely receive a lower score that a simple game with a good web application behind it. I'm not saying you guys can't do this project, but keep these thoughts in mind when deciding your priorities. Examples of components that would well represent what we are looking for: profiles, friending, private messages, leaderboards, and chat rooms, saving game state, cosmetic market with ratings, etc. Of course, once you feel you have a sufficiently complex, well polished web application, you can obviously spend as much time developing the game as you want. Node.js is fine, I used it for my project. Socket.io is also the logical choice due to the necessity for realtime communication, and I would have suggested it if you didn't mention it. It's good to divide responsibilities to each member, but each member should understand the rest of the components well enough to debug and contribute to them. Make sure to develop good interfaces between components early on.

### Charlie
Hi Roy, Alan, Michael, and Ahmed,

I like the idea of a project based on real-time, collaborative interactions among the site users.  I am very concerned, though, by the current nebulous description of your interactive game.  The success of your project will depend on the ability of the players to have rich, real-time interactions on the site, and in its current form those rich interactions can only be developed after the core ideas of the game are designed and implemented, and the richness of the real-time interactions will be inherently limited by the richness of your game design and implementation.

I strongly recommend---but not quite insist---that you find a way to remove, or otherwise reduce the risk of game design from affecting the outcome of your project.  One possible solution is to base your game itself on some existing open source implementation of an interactive game, and have the focus of your project be on implementing a real-time, interactive version of that game for the web.  

Overall I think it is likely that your application is an appropriate size for a four-person project, and that if you encounter routine difficulties you will be able to adapt the scope of your project as needed.

I strongly agree with your choice of node.js  for collaborative real-time interactions, rather than using Django.   Python's---and therefore Django's---support for those type of interactions is markedly worse than with node.js.

### Shannon
This seems like an appropriately scoped project for the course.

However, there are a few drawbacks, the main one being that I want to warn you that you should definitely get the game design down early since this will no doubt shape your design of the application. You don't want to be worrying about the design or rules of the game when you are implementing the application.

Another hazard is that you should make sure that each member of the team has ample experience with the final code of the project. In particular, it's easy, when designing a game, to have a member or two just focusing on the game itself rather than implementation, and that will limit that member's knowledge gained as well as increase the workload on the other teammates.

---

To view this file with formatting, visit the following page: https://github.com/CMU-Web-Application-Development/Team94/blob/master/feedback/proposal.md
