using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Algorithm;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.IO;

namespace Fixturizer
{
        public class GeospatialValueConverter : IValueConverter
        {
                private List<Type> _supportedTypes = new List<Type> {
                        typeof(IGeometry),
                        typeof(ICurve),
                        typeof(IEnvelope),
                        typeof(IGeometryCollection),
                        typeof(ILineString),
                        typeof(ILinearRing),
                        typeof(IMultiCurve),
                        typeof(IMultiLineString),
                        typeof(IMultiPoint),
                        typeof(IMultiPolygon),
                        typeof(IPoint),
                        typeof(IPolygon),
                        typeof(ISurface)
                };

                private PrecisionModel _precisionModel;
                private IGeometryFactory _geometryFactory;
                private WKTReader _reader;
                
                public GeospatialValueConverter()
                {
                        _precisionModel = new PrecisionModel(1);
                        _geometryFactory = new GeometryFactory(_precisionModel, 0);
                        _reader = new WKTReader(_geometryFactory);
                }

                public IList<Type> SupportedTypes
                {
                        get { return _supportedTypes.AsReadOnly(); }
                }

                public object Convert(JValue value)
                {
                        if (value.Type != JTokenType.String)
                        {
                                throw new UnsupportedTypeException();
                        }

                        var raw = (string)value;
                        return _reader.Read(raw);
                }
        }
}
