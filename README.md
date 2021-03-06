# Integrating Blazor with an existing MVC Core 3.1 app
This repo starts with an out-of-the-box MVC Core app and walks you through integrating Blazor Server. It is shockingly easy.

I did this in a large production system and it is working nicely. They co-exist beautifully... This was the easiest migration to a next gen technology I've ever experienced.

# What do I need to know?
Make sure you take a close look at the first few commit notes. Here's a rundown:

 - In the [first commit](https://github.com/kinetiq/Blazor.WithMVC/tree/c6f48a1cd39f569cfee04a8aa2427a719d73b7b6), I bring in an out-of-the-box MVC Core site. 
 - In the [second commit (Step 1)](https://github.com/kinetiq/Blazor.WithMVC/tree/bba2a4f324d0c6153914072de3d43eea22b52d0f), I get Blazor working.
 - After that, in the [third commit (Step 2)](https://github.com/kinetiq/Blazor.WithMVC/tree/392e3ed9788d4abce06cd06fa15bedbf29e28929), I show a configuration I prefer where Blazor runs under YourSite.com/Blazor/, which prevents routing conflicts and is even less invasive than usual. This setup allows MVC to continue handling the default route - normally Blazor takes over the default route, and I wanted my MVC app to keep handling that.

You might be fine with Step 1. For me, I wanted to make absolutely sure my Blazor components were isolated and I didn't want to re-work my default page, so I needed Step 2.

# Why Blazor Server and not Blazor Client / WASM?
When working with an existing MVC app, Blazor Server is the clear choice. The migration is easy and you can leverage existing stlyes and code. WASM is also an amazing technology, 
but since Blazor Client runs in the browser, it is a fundamentally different paradigm. The server needs to provide an API for data access. Some projects may have that, but for 
most of us, creating an API is a lot more disruptive than what I've outlined here.

# What else can I expect once I have this working? #
## Data access might need some tweaking ##
Unlike the vanilla web, which is stateless, Blazor Server maintains persistent state and a live connection to the browser via SignalR. This is magic and it makes web development a 
lot simpler. However, the recommendations for data access are a little different. My MVC project used EF. If yours does too, you should skim this: https://docs.microsoft.com/en-us/aspnet/core/blazor/blazor-server-ef-core?view=aspnetcore-3.1

TL;DR though: none of the DI options you used in MVC are appropriate here, and the best thing is to have very short-lived DbContexts. You could new them up on-demand, but it's 
better to make a factory that configures the DbContext for you.

It's also worth noting that DbContexts do not maintain a live connection to the database, they connect as needed. But they also remember the state of entities you load,
and keeping that around indefinitely can lead to some very confusing problems.
## You might need to re-write some of your nav as blazor components ##
My site's menu has some dynamicism including a search box, and was implemented as a series of partial views. I ended up creating a version of it that is a blazor component.
I do not like duplication, but in this case the trade-off is something I can live with. Maybe I will discover a better way. For instance, it is possible to embed blazor
components in MVC views, so I could scrap the old one and use the blazor nav everywhere. 
## As long as you aren't working at uber-scale, you don't need to be too concerned about scalability ##
First off: one Standard D1 V2 instance on Azure with 3.5 GB of memory has been shown to handle 5,000 concurrent connections. Each user takes about ~273 KB, depending on the size of your state (https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server?view=aspnetcore-3.1, https://devblogs.microsoft.com/aspnet/blazor-server-in-net-core-3-0-scenarios-and-performance/). To me, that means this is usable for 90% of the sites in the world. 

If you are highly likely to operate at uber-scale, this is probably not the technology for you. But if you live in that world, you already know that most of those projects use open source stacks because they are so much cheaper at that level. And you probably have a large team of co-workers who thinks about nothing but scale.

Also, there are interesting options for pushing Blazor Server even further. Azure has SignalR Server, which can hold all your websocket connections for you, decreasing the per-user server load significantly. 

## The open source ecosystem is a responsible 14 year old: not immature, but I wouldn't leave them alone with my beer ##
MVC launched in 2009 and we now enjoy a vibrant ecoysystem. JS also has a library for everything. Or 10. In Blazor, the ecoystem seems to be... Sufficient. I needed to build
something with n-level drag and drop, and it was hard to find a good library, even among the paid component libraries. Ultimately I am pleased to say I did not have to write 
this myself, I _did_ find something that worked, but I am noticing that in JS I would have had my pick between sortableJS, dragula, and two dozen more. 

At the same time, there are multiple great paid component libraries from big name vendors, all your C# libraries still work, and the lack of a truly vibrant OSS ecosystem
is an opportunity for glory. Get in there and build something!



