MONO=/usr/bin/mono
DMCS=/usr/bin/dmcs
NSJSON=vendor/Newtonsoft.Json.Net35.dll
NUNITCORE=vendor/nunit-2.6/bin/lib/nunit.core.dll
NUNITFW=vendor/nunit-2.6/bin/nunit.framework.dll
NUNITCONSOLE=vendor/nunit-2.6/bin/nunit-console.exe

library: src/Loader.cs 
	$(DMCS) /out:dist/Fixturizer.dll /t:library /r:$(NSJSON) src/*.cs
	cp $(NSJSON) dist

runner: Runner.cs
	$(DMCS) /out:dist/Runner.exe /r:$(NSJSON) Runner.cs src/*.cs
	cp $(NSJSON) dist

tests: src/Loader.cs test/TestLoader.cs
	$(DMCS) /out:dist/Fixturizer.Test.dll /t:library /r:$(NSJSON) /r:$(NUNITCORE) /r:$(NUNITFW) src/*.cs test/*.cs
	cp $(NSJSON) $(NUNITCORE) $(NUNITFW) dist
	$(MONO) $(NUNITCONSOLE) dist/Fixturizer.Test.dll

all: library runner tests

