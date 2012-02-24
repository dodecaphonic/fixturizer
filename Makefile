MONO=/usr/bin/mono
DMCS=/usr/bin/dmcs
NSJSON=vendor/Newtonsoft.Json.Net35.dll
NUNITCORE=vendor/nunit-2.6/bin/lib/nunit.core.dll
NUNITFW=vendor/nunit-2.6/bin/nunit.framework.dll
GEOAPI=vendor/GeoAPI.dll
NTSUITE=vendor/NetTopologySuite.dll
IESI=vendor/Iesi.Collections.dll
NUNITCONSOLE=vendor/nunit-2.6/bin/nunit-console.exe
OUTPUTLIB=dist/Fixturizer.dll

clean:
	rm -f dist/*

library: src/*.cs
	$(DMCS) /out:$(OUTPUTLIB) /t:library /r:$(NSJSON) /r:$(NTSUITE) /r:$(GEOAPI) /r:$(IESI) src/*.cs
	cp $(NSJSON) $(GEOAPI) $(NTSUITE) $(IESI) dist

tests: library test/*.cs
	$(DMCS) /out:dist/Fixturizer.Test.dll /t:library /r:$(OUTPUTLIB) /r:$(GEOAPI) /r:$(NSJSON) /r:$(NTSUITE) /r:$(NUNITCORE) /r:$(NUNITFW) test/*.cs
	cp $(NUNITCORE) $(NUNITFW) dist
	$(MONO) $(NUNITCONSOLE) dist/Fixturizer.Test.dll

all: clean library tests

