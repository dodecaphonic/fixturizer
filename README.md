# Fixturizer

A dollop of data for your tests.

## Goals

 * To make loading bucketfuls of data for test setup really easy
 * To deal only with the application's object graph, skipping any concerns with databases and such
 * To create GeoAPI Geometries

## Fixtures

Fixturizer's data files are written in JSON format. It was chosen for its unceremonious notation, mostly, and good library support (via [JSON.NET][json.net]) made it a favorable implementation choice. Following this fact, all datafiles must end with a *".json"* extension.

### Format

A fixture consists of a JSON array of objects that are to be mapped to a single .NET type. Suppose you have the following .NET class in your program:

    class TodoItem
    {
        public string Description { get; set; }
        public DateTime Due { get; set; }
        public int Priority { get; set; }
    }

If you were to write a set of data fixtures for it, your file would look something like:

    [
      {
          "Description": "Take out the trash before my wife kills me",
          "Due": "/Date(1330121559915)/",
          "Priority": 5
      },
      {
          "Description": "Make dentist appointment",
          "Priority": 2
      },
      {

          "Description": "Win Mr. Universe",
          "Due": "/Date(1361747732000)/",
          "Priority": 5
      }
    ]

As you can see, the objects' properties match _TodoItem_'s, a characteristic that makes writing data fixtures in Fixturizer pretty straightforward.

#### Nested Types

One common situation is building objects with other nested objects. You need to build a complex object graph without getting pieces from different data sources and going about manually filling the slots up, so I felt this should be a pretty simple use case.

For the sake of an exemple, let's say you want to attach a file to a _TodoItem_. You could define _Attachment_ like this:

    class Attachment
    {
        public string Path { get; set; }
    }

Extend TodoItem so it has a field named "Attachment" with type _Attachment_ and expand the fixture definition accordingly:

    [
      {
          "Description": "Take out the trash before my wife kills me",
          "Due": "/Date(1330121559915)/",
          "Priority": 5,
          "Attachment": {
            "Path": "/my/secret/files/menacing_wife.png"
          }
      }
    ]

And voilà: when you load this _TodoItem_, its _Attachment_ will be there waiting. To be clear, your nested types need not be part of your application specifically: they must simply be available for instantiation at runtime and have public, settable properties matching whatever you defined in your fixture.

##### Collections

Nesting doesn't stop at single objects. You can load a bunch of them, really, and have a nice collection filled up for you when the magic happens..

Let's say you want to test a todo list. You could define it as an object containing a set of the previously-described _TodoItem_:

    using System.Collections.Generic;
    
    class TodoList
    {
        public string Name { get; set; }
        public IList<TodoItem> Items { get; set; }
    }

Your fixture's property must point to a JSON array containing objects matching the nested type. For our _TodoItem_s, it would look something like this:

    [
      {
        "Name": "A man with a plan",
        "Items": [
          {
            "Description": "Take out the trash before my wife kills me",
            "Due": "/Date(1330121559915)/",
            "Priority": 5,
            "Attachment": {
              "Path": "/my/secret/files/menacing_wife.png"
            }
          },
          {
            "Description": "Make dentist appointment",
            "Priority": 2
          },
          {
            "Description": "Win Mr. Universe",
            "Due": "/Date(1361747732000)/",
            "Priority": 5
          }
        ]
      }
    ]

###### Limitations

Currently the loading of Collections only works for System.Collections.Generic.IList<> and Iesi.Collections.ISet<>. If you need more, please go on and implement it. I'll be really glad.

#### Geometries

One of the leading motivations to write Fixturizer was the need to load GeoAPI geometries into the database without going through the hula-hoops of APIs and instantiations and incantations. To make matters simple, geometries are written in [WKT][wkt], using a plain JSON string. [GeoJSON][geojson] felt a bit too precious for my purposes, but correctness may lead Fixturizer that way. Time will tell.

To make a convoluted example, take your new-fangled great idea: a competitor to Google Maps with 4D interfaces and direct connections to the user's basal ganglia, so each interaction releases a rush of dopamine. You're really excited, but you know you have to start small and build from there, so you come up with a Placemark class:

    using GeoAPI.Geometries;
    
    class Placemark
    {
        public IGeometry Position { get; set; }
    }

When you're writing your model tests, your fixture will look like this:

    [
      {
         "Position": "POINT(1 0)"
      },
      {
         "Position": "LINESTRING(0 0, 0 1, 1 1)"
      }
    ]

When you deserialize the objects, everything will be compliant with _IGeometry_ and the birds outside your window will chirp in delight.

## Usage

Fixturizer's main star is _Loader_, the class you'll interact with in your tests. It takes a directory in its constructor &em; this is where your fixtures are located.

    var loader = new Fixturizer.Loader("/my/fixtures");

To load the _TodoList_ fixture you defined above and saved with the creative name of "todolists.json", use the _Load<T>_ method:

    // todoLists will be an instance of IList<TodoList>
    var todoLists = loader.Load<TodoList>("todolists"); // ".json" is implicit
    var todoList = todoLists.First();

From then on you can interact with it normally:

    Console.WriteLine(todoList.Name); # => "A man with a plan"


[wkt]: http://en.wikipedia.org/wiki/Well-known_text
[geojson]: http://en.wikipedia.org/wiki/GeoJSON
[json.net]: http://james.newtonking.com/pages/json-net.aspx
