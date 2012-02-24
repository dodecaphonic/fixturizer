# Fixturizer

A dollop of data for your tests.

## Goals

 * To make loading bucketfuls of data for test setup really easy
 * To deal only with the application's object graph, skipping any concerns with databases and such
 * To create GeoAPI Geometries

## Fixtures

Fixturizer's data files are written in JSON format. The non-ceremonious ways of the notation and good library support (via Newtonsoft.Json) made it a top-notch implementation choice. All files have to end with a *".json"* extension.

### Format

A fixture consists of a JSON array of objects to be mapped to a single .NET type (this may change in the future). You place data in the target type using the latter's public property names.

Suppose you have the following .NET class:

    class TodoItem
    {
        public string Description { get; set; }
        public DateTime Due { get; set; }
        public int Priority { get; set; }
    }

If you were to write a set of data fixtures for it, your file would look something like this:

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

This is pretty straightforward, I think.

#### Nested Types

Let's say you want to attach a file to a _TodoItem_. You could define _Attachment_ like this:

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


##### Collections

Let's say you want to test a todo list. You could define it as an object containing a set of the previously-described TodoItem:

    using System.Collections.Generic;
    
    class TodoList
    {
        public string Name { get; set; }
        public IList<TodoItem> Items { get; set; }
    }

Your fixture file would look something like this:

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

You can easily notice that _Items_ is define as a JSON Array with objects that conform to _TodoItem_'s interface.

###### Limitations

Currently the loading of Collections only works for System.Collections.Generic.IList<> and Iesi.Collections.ISet<>. If you need more, please go on and implement it.

#### Geometries

One of the leading motivations to write Fixturizer was the need to load GeoAPI geometries into the database without going through the hula-hoops of APIs and instantiations and incantations. To make matters simple, geometries are written in WKT, using a plain JSON string. GeoJSON may be more appropriate, but for now WKT is enough.

To make a convoluted example, take your new-fangled great idea: a competitor to Google Maps with 4D interfaces and direct connections to the user's basal ganglia, so each interaction releases a rush of dopamine. You're really excited, but you know you have to start small and build from there, so you define a Placemark class:

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


## Usage

Fixturizer's main star is _Loader_, the class you'll interact with in your tests. It takes a directory in its constructor &em; this is where your fixtures are located.

    var loader = new Fixturizer.Loader("/my/fixtures");

To load the _TodoList_ fixture you defined above, which you saved with the creative name of "todolist.json", use the _Load_ method:

    // todoLists will be an instance of IList<TodoList>
    var todoLists = loader.Load<TodoList>("todolists"); // ".json" is implicit
    var todoList = todoLists.First();

From then on you can interact with it normally:

    Console.WriteLine(todoList.Name); # => "A man with a plan"
 
