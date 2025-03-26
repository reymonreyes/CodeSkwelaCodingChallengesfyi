using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace JSONParser.UnitTests
{
    public class JsonParserTests
    {
        [Fact]
        public void Parse_ShouldReturnZeroAndValidMessage_WhenJsonIsValid()
        {
            var parser = new JSONParser.JsonParser();
            
            Assert.Equal(0, parser.IsValid("{}"));
        }

        [Fact]
        public void Parse_ShouldReturnOneAndInvalidMessage_WhenJsonIsInvalid()
        {
            var parser = new JSONParser.JsonParser();

            Assert.Equal(1, parser.IsValid(""));
        }

        [Fact]
        public void Parse_ShouldReturnOneAndInvalidMessage_WhenJsonIsInvalid_Step2_invalid()
        {
            var parser = new JSONParser.JsonParser();
            var json = """{"key": "value",}""";           

            Assert.Equal(1, parser.IsValid(json));
        }

        [Fact]
        public void Parse_ShouldReturnOneAndInvalidMessage_WhenJsonIsInvalid_Step2_invalid2()
        {
            var parser = new JSONParser.JsonParser();
            var json = """
                {
                  "key": "value",
                  key2: "value"
                }
                """;

            Assert.Equal(1, parser.IsValid(json));
        }

        [Fact]
        public void Parse_ShouldReturnOneAndInvalidMessage_WhenJsonIsInvalid_Step2_valid()
        {
            var parser = new JSONParser.JsonParser();
            var json = """{"key": "value"}""";

            Assert.Equal(0, parser.IsValid(json));
        }

        [Fact]
        public void Parse_ShouldReturnOneAndInvalidMessage_WhenJsonIsInvalid_Step2_valid2()
        {
            var parser = new JSONParser.JsonParser();
            var json = """
                {
                  "key": "value",
                  "key2": "value"
                }
                """;

            Assert.Equal(0, parser.IsValid(json));
        }

        [Fact]
        public void Parse_ShouldReturnOneAndInvalidMessage_WhenJsonIsInvalid_Step3_invalid()
        {
            var parser = new JSONParser.JsonParser();
            var json = """
                {
                  "key1": true,
                  "key2": False,
                  "key3": null,
                  "key4": "value",
                  "key5": 101
                }
                """;

            Assert.Equal(1, parser.IsValid(json));
        }

        [Fact]
        public void Parse_ShouldReturnOneAndInvalidMessage_WhenJsonIsInvalid_Step3_valid()
        {
            var parser = new JSONParser.JsonParser();
            var json = """
                {
                  "key1": true,
                  "key2": false,
                  "key3": null,
                  "key4": "value",
                  "key5": 101
                }
                """;

            Assert.Equal(0, parser.IsValid(json));
        }

        [Fact]
        public void Parse_ShouldReturnOneAndInvalidMessage_WhenJsonIsInvalid_Step4_invalid()
        {
            var parser = new JSONParser.JsonParser();
            var json = """
                {
                  "key": "value",
                  "key-n": 101,
                  "key-o": {
                    "inner key": "inner value"
                  },
                  "key-l": ['list value']
                }
                """;

            Assert.Equal(1, parser.IsValid(json));
        }

        [Fact]
        public void Parse_ShouldReturnOneAndInvalidMessage_WhenJsonIsInvalid_Step4_valid()
        {
            var parser = new JSONParser.JsonParser();

            var json = """{"k"ey": "\"value"}""";
            Assert.Equal(1, parser.IsValid(json));
        }

        [Fact]
        public void IsValidStringKey_ShouldReturnInvalid_1()
        {
            var jsonParser = new JSONParser.JsonParser();
            var testStr = """
                {
                    "k:
                }
                """;
            var result = jsonParser.IsValidStringKey(testStr);
            
            Assert.Equal(1, result);
        }

        [Fact]
        public void IsValidStringKey_ShouldReturnInvalid_2()
        {
            var jsonParser = new JSONParser.JsonParser();
            var testStr = "{\"\"\":\"\"}";
            var result = jsonParser.IsValidStringKey(testStr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void IsValidStringKey_ShouldReturnInvalid_3()
        {
            var jsonParser = new JSONParser.JsonParser();
            var testStr = """
                {
                    "k"":
                }
                """;
            var result = jsonParser.IsValidStringKey(testStr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void IsValidStringKey_ShouldReturnInvalid_4()
        {
            var jsonParser = new JSONParser.JsonParser();
            var testStr = """
                {
                    "k\":
                }
                """;
            var result = jsonParser.IsValidStringKey(testStr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void IsValidStringKey_ShouldReturnInvalid_5()
        {
            var jsonParser = new JSONParser.JsonParser();
            var testStr = """
                {
                    "k""\":
                }
                """;
            var result = jsonParser.IsValidStringKey(testStr);

            Assert.Equal(1, result);
        }

        [Fact]
        public void IsValidStringKey_ShouldReturnInvalid_6()
        {
            var jsonParser = new JSONParser.JsonParser();
            var testStr = """
                {
                    "k\"\"\"":""
                }
                """;
            var result = jsonParser.IsValidStringKey(testStr);

            Assert.Equal(1, result);
        }        

        [Fact]
        public void IsValid_KeyShouldBeInvalid()
        {
            var parser = new JsonParser();

            var json = """"
                    {
                        "\"\"\u 01":""
                    }
                """";
            var json2 = """"
                    {
                        "\"\"\ "":""
                    }
                """";

            Assert.Equal(1, parser.IsValid(json));
            Assert.Equal(1, parser.IsValid(json2));
        }

        [Fact]
        public void IsValid_KeyShouldBeValid()
        {
            var parser = new JsonParser();

            var json = """"
                    {
                        "\"\"\u0061abcd":""
                    }
                """";

            var json2 = """
                {
                    "ke:y": "val"
                }
            """;
            
            var json3 = """
                {
                    "ke:::::y": "val"
                }
            """;

            Assert.Equal(0, parser.IsValid(json));
            Assert.Equal(0, parser.IsValid(json2));
            Assert.Equal(0, parser.IsValid(json3));
        }

        [Fact]
        public void IsValidHexValue_ShouldBeValid()
        {
            var parser = new JsonParser();
            var test = "009E";
            
            var result = parser.IsValidHexValue(test, 0);

            Assert.True(result);
        }

        [Fact]
        public void IsValidHexValue_ShouldBeInvalid()
        {
            var parser = new JsonParser();

            Assert.False(parser.IsValidHexValue("09E", 0));
            Assert.False(parser.IsValidHexValue("09EG", 0));
        }

        [Fact]
        public void IsValid_StringTokenShouldBeInvalid()
        {
            var parser = new JsonParser();

            var strHasNoClosingDoubleQuote = """
                {
                    "key":"
                }
                """;

            var strHasNoPairOfDoubleQuotes = """
                {
                    "key":
                }
                """;

            var strHasNoEnclosingBraces = """
                {
                    "key":
                
                """;

            var strHasDoubleQuoteButWithoutEnclosingBraces = """
                {
                    "key":""
                
                """;
            var json5 = """"["a",]"""";
            var json6 = """"["a": false]"""";

            Assert.Equal(1, parser.IsValid(strHasNoClosingDoubleQuote));
            Assert.Equal(1, parser.IsValid(strHasNoPairOfDoubleQuotes));
            Assert.Equal(1, parser.IsValid(strHasNoEnclosingBraces));
            Assert.Equal(1, parser.IsValid(strHasDoubleQuoteButWithoutEnclosingBraces));
            Assert.Equal(1, parser.IsValid(json5));
            Assert.Equal(1, parser.IsValid(json6));
        }

        [Fact]
        public void IsValid_StringTokenShouldBeValid()
        {
            var parser = new JSONParser.JsonParser();

            var strWithValidToken = """{"key": ""}""";
            var strValidTokenWithValue = """{"key": "value"}""";
            var json3 = """{"special": "`1~!@#$%^&*()_+-={':[,]}|;.</>?"}""";
            var json4 = """
                {
                    "backslash": "\\", "controls": "\b\f\n\r\t","slash": "/ & \/",
                    "alpha": "abcdefghijklmnopqrstuvwyz",
                    "ALPHA": "ABCDEFGHIJKLMNOPQRSTUVWYZ",
                    "digit": "0123456789",
                    "0123456789": "digit",
                    "special": "`1~!@#$%^&*()_+-={':[,]}|;.</>?"
                }
                """;
            var json5 = """{"\/\\\"\uCAFE\uBABE\uAB98\uFCDE\ubcda\uef4A\b\f\n\r\t`1~!@#$%^&*()_+-=[]{}|;:',./<>?":"A key can be any string"}""";
            var json6 = """{"as\"qw:df:gh":""}""";

            Assert.Equal(0, parser.IsValid(strWithValidToken));
            Assert.Equal(0, parser.IsValid(strValidTokenWithValue));
            Assert.Equal(0, parser.IsValid(json3));
            Assert.Equal(0, parser.IsValid(json4));
            Assert.Equal(0, parser.IsValid(json5));
            Assert.Equal(0, parser.IsValid(json6));
        }

        [Fact]
        public void IsStringValueValidJson_ValueShouldBeInvalid()
        {
            var parser = new JSONParser.JsonParser();

            var strWithUnescapedCharacters = @"\";
            var strWithUnescapedCharacters2 = @"\t\";
            var strWithUnescapedCharacters3 = """\r\\\""";
            var strWithUnescapedCharacters4 = @"\u";
            var strWithUnescapedCharacters5 = """"
\""
"""";

            Assert.False(parser.IsStringValueValidJson(strWithUnescapedCharacters));
            Assert.False(parser.IsStringValueValidJson(strWithUnescapedCharacters2));
            Assert.False(parser.IsStringValueValidJson(strWithUnescapedCharacters3));
            Assert.False(parser.IsStringValueValidJson(strWithUnescapedCharacters4));
            Assert.False(parser.IsStringValueValidJson(strWithUnescapedCharacters5));
        }

        [Fact]
        public void IsStringValueValidJson_ValueShouldBeValid()
        {
            var parser = new JSONParser.JsonParser();

            var strWithNormalStrings = "this is a value";
            var strWithNumbers = "id: 1234";
            var strWithEscapedCharacters = """
\t\r\n\"
""";
            var strWithHexValues = @"data[\u1324]";
            var strWithNormalStrings2 = """
                some value here
                """;

            Assert.True(parser.IsStringValueValidJson(strWithNormalStrings));
            Assert.True(parser.IsStringValueValidJson(strWithNumbers));
            Assert.True(parser.IsStringValueValidJson(strWithEscapedCharacters));
            Assert.True(parser.IsStringValueValidJson(strWithHexValues));
            Assert.True(parser.IsStringValueValidJson(strWithNormalStrings2));
        }

        [Fact]
        public void IsValid_StringValueShouldBeInvalid()
        {
            var parser = new JSONParser.JsonParser();

            var strWithUnescapedCharacters = """
                {
                    "key": "\"
                }
            """;

            var strWithIncompleteHexValue = """
                {
                    "key": "\u"
                }
            """;

            Assert.Equal(1, parser.IsValid(strWithUnescapedCharacters));
            Assert.Equal(1, parser.IsValid(strWithIncompleteHexValue));
        }

        [Fact]
        public void IsValid_StringValueShouldBeValid()
        {
            var parser = new JSONParser.JsonParser();

            var strWithValidValue = """
                {
                    "key": "this is a valid value"
                }
            """;

            var strWithEscapedCharacters = """
                {
                    "key": "data: [\u0002]"
                }
            """;

            Assert.Equal(0, parser.IsValid(strWithValidValue));
            Assert.Equal(0, parser.IsValid(strWithEscapedCharacters));
        }

        [Fact]
        public void IsValid_MultipleKeyValueAsStringShouldBeInvalid()
        {
            var parser = new JSONParser.JsonParser();

            var strWithEmptyNextKeyValue = """{"key": "val",}""";

            var strWithMissingValue = """{"key":"val","key2":}""";

            var strWithMissingKey = """
                {
                    : "val",
                    "key2":""
                }
            """;

            var strWithMissingKey2 = """
                {
                    "key1": "val",
                    :""
                }
            """;

            var strWithInvalidKey = """
                {
                    : "val",
                    "key2":""
                }
            """;

            var strWithInvalidKey2 = """
                {
                    "key1" : "val",
                    :""
                }
            """;

            var strWithInvalidKey3 = """{"key": "val",123}""";

            Assert.Equal(1, parser.IsValid(strWithEmptyNextKeyValue));
            Assert.Equal(1, parser.IsValid(strWithMissingValue));
            Assert.Equal(1, parser.IsValid(strWithMissingKey));
            Assert.Equal(1, parser.IsValid(strWithMissingKey2));
            Assert.Equal(1, parser.IsValid(strWithInvalidKey));
            Assert.Equal(1, parser.IsValid(strWithInvalidKey2));
            Assert.Equal(1, parser.IsValid(strWithInvalidKey3));
        }

        [Fact]
        public void IsValid_MultipleKeyValueAsStringShouldBeValid()
        {
            var parser = new JSONParser.JsonParser();

            var strWithMultipleKeyValues = """{"key":"val","id":"1234"}""";

            Assert.Equal(0, parser.IsValid(strWithMultipleKeyValues));
        }

        [Fact]
        public void IsValid_ValueAsNumberZeroToNineMustBeValid()
        {
            var parser = new JSONParser.JsonParser();

            var jsonWithAValidZeroValue = """{"key": 0}""";
            var jsonWithAValidOneToNineValue = """{"key": 1}""";
            var jsonWithAValidOneToNineValues = """{"key": 12340}""";
            var json4 = """{"integer": 1234567890}""";
            var json5 = """{"real": -9876.543210}""";
            var json6 = """{"e": 0.123456789e-12}""";
            var json7 = """{"E": 1.234567890E+34}""";
            var json8 = """{"":  23456789012E66}""";

            Assert.Equal(0, parser.IsValid(jsonWithAValidZeroValue));
            Assert.Equal(0, parser.IsValid(jsonWithAValidOneToNineValue));
            Assert.Equal(0, parser.IsValid(jsonWithAValidOneToNineValues));
            Assert.Equal(0, parser.IsValid(json4));
            Assert.Equal(0, parser.IsValid(json5));
            Assert.Equal(0, parser.IsValid(json6));
            Assert.Equal(0, parser.IsValid(json7));
            Assert.Equal(0, parser.IsValid(json8));
        }

        [Fact]
        public void IsValid_ValueAsNumberZeroToNineMustBeInvalid()
        {
            var parser = new JSONParser.JsonParser();

            var jsonWithAnInvalidZeroValue = """{"key": 01}""";
            var jsonWithSpaceBetweenZeroAndOneToNineValue = """{"key": 0 1}""";
            var jsonWithSpaceBetweenOneToNineValue = """{"key": 1 1 1}""";
            var json4 = """{"key": 0.   1}""";
            
            Assert.Equal(1, parser.IsValid(jsonWithAnInvalidZeroValue));
            Assert.Equal(1, parser.IsValid(jsonWithSpaceBetweenZeroAndOneToNineValue));
            Assert.Equal(1, parser.IsValid(jsonWithSpaceBetweenOneToNineValue));
            Assert.Equal(1, parser.IsValid(json4));
        }

        [Fact]
        public void IsValid_ValueAsFractionMustBeInvalid()
        {
            var parser = new JSONParser.JsonParser();
            var jsonFractionWithSpaceBetweenDigits = """
                {
                    "key": 1.12 34
                }
            """;
            var jsonFractionWithoutWholeNumber = """
                {
                    "key": .1
                }
            """;
            var jsonFractionWithMultipleDecimalPoints = """
                {
                    "key": 1.12.34
                }
            """;

            var jsonFractionWithOnlyASingleDecimalPoint = """
                {
                    "key": .
                }
            """;

            Assert.Equal(1, parser.IsValid(jsonFractionWithSpaceBetweenDigits));
            Assert.Equal(1, parser.IsValid(jsonFractionWithoutWholeNumber));
            Assert.Equal(1, parser.IsValid(jsonFractionWithMultipleDecimalPoints));
            Assert.Equal(1, parser.IsValid(jsonFractionWithOnlyASingleDecimalPoint));
        }

        [Fact]
        public void IsValid_ValueAsFractionMustBeValid()
        {
            var parser = new JSONParser.JsonParser();
            var jsonWithFractionValue = """
                {
                    "key": 1.22323
                }
            """;

            Assert.Equal(0, parser.IsValid(jsonWithFractionValue));
        }

        [Fact]
        public void IsValid_ValueAsExponentMustBeValid()
        {
            var parser = new JSONParser.JsonParser();
            var jsonDecimalNumberWithExponentWithoutSign = """
                {
                    "key": 1.1E2
                }
            """;
            var jsonDecimalNumberWithExponentWithPositiveSign = """
                {
                    "key": 1.1E+2
                }
            """;
            var jsonDecimalNumberWithExponentWithNegativeSign = """
                {
                    "key": 1.1E-2
                }
            """;
            var jsonDecimalNumberWithSubExponentWithPositiveSign = """
                {
                    "key": 1.1e+2
                }
            """;
            var jsonDecimalNumberWithSubExponentWithNegativeSign = """
                {
                    "key": 1.1e-2
                }
            """;

            Assert.Equal(0, parser.IsValid(jsonDecimalNumberWithExponentWithoutSign));
            Assert.Equal(0, parser.IsValid(jsonDecimalNumberWithExponentWithPositiveSign));
            Assert.Equal(0, parser.IsValid(jsonDecimalNumberWithExponentWithNegativeSign));
            Assert.Equal(0, parser.IsValid(jsonDecimalNumberWithSubExponentWithPositiveSign));
            Assert.Equal(0, parser.IsValid(jsonDecimalNumberWithSubExponentWithNegativeSign));
        }

        [Fact]
        public void IsValid_ValueAsExponentMustBeInvalid()
        {
            var parser = new JSONParser.JsonParser();
            
            var jsonDecimalNumberWithSubExponentNotFollowedByAnyDigit = """
                {
                    "key": 1.1E
                }
            """;
            var jsonDecimalNumberWithInvalidExponentSymbol = """
                {
                    "key": 1.1A-2
                }
            """;
            
            Assert.Equal(1, parser.IsValid(jsonDecimalNumberWithSubExponentNotFollowedByAnyDigit));
            Assert.Equal(1, parser.IsValid(jsonDecimalNumberWithInvalidExponentSymbol));
        }

        [Fact]
        public void IsValid_ValueAsBooleanMustBeValid()
        {
            var parser = new JSONParser.JsonParser();
            var jsonWithValidTrueBoolean = """{"key": true}""";
            var jsonWithValidFalseBoolean = """{"key": false}""";

            Assert.Equal(0, parser.IsValid(jsonWithValidTrueBoolean));
            Assert.Equal(0, parser.IsValid(jsonWithValidFalseBoolean));
        }

        [Fact]
        public void IsValid_ValueAsBooleanMustBeInvalid()
        {
            var parser = new JSONParser.JsonParser();
            var jsonWithInvalidBoolean = """{"key": tru}""";
            var jsonWithInvalidBoolean2 = """{"key": fals}""";
            var jsonWithInvalidBoolean3 = """{"key": True}""";
            var jsonWithInvalidBoolean4 = """{"key": False}""";

            Assert.Equal(1, parser.IsValid(jsonWithInvalidBoolean));
            Assert.Equal(1, parser.IsValid(jsonWithInvalidBoolean2));
            Assert.Equal(1, parser.IsValid(jsonWithInvalidBoolean3));
            Assert.Equal(1, parser.IsValid(jsonWithInvalidBoolean4));
        }

        [Fact]
        public void IsValid_ValueAsNullMustBeValid()
        {
            var parser = new JSONParser.JsonParser();
            var jsonWithValidNull = """{"key": null}""";

            Assert.Equal(0, parser.IsValid(jsonWithValidNull));
        }

        [Fact]
        public void IsValid_ValueAsNullMustBeInvalid()
        {
            var parser = new JSONParser.JsonParser();
            var jsonWithInvalidNull = """{"key": nulx}""";
            var jsonWithInvalidNull2 = """{"key": nul}""";

            Assert.Equal(1, parser.IsValid(jsonWithInvalidNull));
            Assert.Equal(1, parser.IsValid(jsonWithInvalidNull2));
        }        

        [Fact]
        public void FindKeyEndIndex_IsValid()
        {
            var parser = new JSONParser.JsonParser();

            var str1 = @"""abc:def:gh"":";
            var str2 = @"""abcdefgh"":";
            var str3 = @""""":";
            var str4 = @"""""   :";

            Assert.True(parser.FindKeyEndIndex(0, ref str1).endIndex >= 0);
            Assert.True(parser.FindKeyEndIndex(0, ref str2).endIndex >= 0);
            Assert.True(parser.FindKeyEndIndex(0, ref str3).endIndex >= 0);
            Assert.True(parser.FindKeyEndIndex(0, ref str4).endIndex >= 0);
        }

        [Fact]
        public void FindKeyEndIndex_IsInvalid()
        {
            var parser = new JSONParser.JsonParser();

            var str1 = @"""abc""";
            var str2 = @"""ab:c""";
            
            Assert.True(parser.FindKeyEndIndex(0, ref str1).endIndex < 0);
            Assert.True(parser.FindKeyEndIndex(0, ref str2).endIndex < 0);
        }        

        [Fact]
        public void IsValid_ObjectMustBeValid()
        {
            var parser = new JSONParser.JsonParser();
            var jsonObject = """{}""";
            var jsonObject2 = """{"key":"val"}""";
            var jsonObject3 = """{"key":"val", "keyNum": 1234}""";
            var jsonObject4 = """{"key":"val", "keyNum": 1234,"keyBoolean":true}""";
            var jsonObject5 = """{"key":"val", "keyNum": 1234,"keyBoolean":true, "keyNull":null}""";
            var jsonObject6 = """{"key":{"key2":"val"}}""";
            var jsonObject7 = """{"key":{"key2":"val", "keyNum": 123}}""";
            var jsonObject8 = """{"key":"val", "key2": {"key3": "val2", "key4": 123}}""";
            var jsonObject9 = """{"key":"val", "key2": {"key3": "val2", "key4": 123, "key5": true}}""";
            var jsonObject10 = """{"key":{"key2":"val"}, "key3": "val"}""";
            var jsonObject11 = """{"key":{"key2":"val"}, "key3": "val", "key4":null}""";
            var jsonObject12 = """{"key":{"key2":"val"}, "key3": "val", "key4":null, "key5":{"key6": true}}""";

            Assert.Equal(0, parser.IsValid(jsonObject));
            Assert.Equal(0, parser.IsValid(jsonObject2));
            Assert.Equal(0, parser.IsValid(jsonObject3));
            Assert.Equal(0, parser.IsValid(jsonObject4));
            Assert.Equal(0, parser.IsValid(jsonObject5));
            Assert.Equal(0, parser.IsValid(jsonObject6));
            Assert.Equal(0, parser.IsValid(jsonObject7));
            Assert.Equal(0, parser.IsValid(jsonObject8));
            Assert.Equal(0, parser.IsValid(jsonObject9));
            Assert.Equal(0, parser.IsValid(jsonObject10));
            Assert.Equal(0, parser.IsValid(jsonObject11));
            Assert.Equal(0, parser.IsValid(jsonObject12));
        }

        [Fact]
        public void IsValid_ObjectMustBeInvalid()
        {
            var parser = new JSONParser.JsonParser();
            var json = """{""";
            var json2 = """{ad""";
            var json3 = """{}a""";
            var json4 = """{"key": {"key": "val"}""";
            var json5 = """{"key": {"key": "val"}}a""";
            var json6 = """{"key": {"key": "val"}""";
            var json7 = """{"key": {"key": "val"},""";
            var json8 = """{"key": {"key": "val"},{""";
            var json9 = """{"key": "val",}""";
            var json10 = """{"key": "val"}}""";
            var json11 = """{,}""";
            var json12 = """{{}}""";
            var json13 = """"{"a" null}"""";

            Assert.Equal(1, parser.IsValid(json));
            Assert.Equal(1, parser.IsValid(json2));
            Assert.Equal(1, parser.IsValid(json3));
            Assert.Equal(1, parser.IsValid(json4));
            Assert.Equal(1, parser.IsValid(json5));
            Assert.Equal(1, parser.IsValid(json6));
            Assert.Equal(1, parser.IsValid(json7));
            Assert.Equal(1, parser.IsValid(json8));
            Assert.Equal(1, parser.IsValid(json9));
            Assert.Equal(1, parser.IsValid(json10));
            Assert.Equal(1, parser.IsValid(json11));
            Assert.Equal(1, parser.IsValid(json12));
            Assert.Equal(1, parser.IsValid(json13));
        }

        [Fact]
        public void IsValid_ArrayMustBeValid()
        {
            var parser = new JSONParser.JsonParser();
            var json = "[]";
            var jsonArray1 = """["value", "value2", 123, null]""";
            var jsonArray2 = """["value", "value2", 123, null, true]""";
            var jsonArray3 = """[456, "value", "value2", 123, null, true]""";
            var jsonArrayWithObject = """[{"key": "val"}]""";
            var jsonObjectWithArray = """{"key1":["val1","val2", 123]}""";
            var jsonNestedArray = """[["val1","val2"], 123]""";
            var jsonNestedArray2 = """[["val1","val2"], 123, [123, "val3"]]""";
            var jsonNestedArray3 = """[["val1","val2",[123,"val4"]], 123, [123, "val3"]]""";
            var json4 = """[{"key1":123,"key2": "val1", "key3":{"key4":[null, true, "val"]}}]""";
            var json5 = """{"key1":[null, true, "val"]}""";
            var json6 = """[{"key3":{"key4":[null, true, "val"]}}]""";
            var json7 = """[[]]""";
            var json8 = """[[[]]]""";
            var json9 = """[[],{}]""";

            Assert.Equal(0, parser.IsValid(json));
            Assert.Equal(0, parser.IsValid(jsonArray1));
            Assert.Equal(0, parser.IsValid(jsonArray2));
            Assert.Equal(0, parser.IsValid(jsonArray3));
            Assert.Equal(0, parser.IsValid(jsonArrayWithObject));
            Assert.Equal(0, parser.IsValid(jsonObjectWithArray));
            Assert.Equal(0, parser.IsValid(jsonNestedArray));
            Assert.Equal(0, parser.IsValid(jsonNestedArray2));//
            Assert.Equal(0, parser.IsValid(jsonNestedArray3));
            Assert.Equal(0, parser.IsValid(json4));
            Assert.Equal(0, parser.IsValid(json5));
            Assert.Equal(0, parser.IsValid(json6));
            Assert.Equal(0, parser.IsValid(json7));
            Assert.Equal(0, parser.IsValid(json8));
            Assert.Equal(0, parser.IsValid(json9));
        }

        [Fact]
        public void IsValid_ArrayMustBeInvalid()
        {
            var parser = new JSONParser.JsonParser();
            var json = """[""";
            var json2 = """["val",  """;
            var json3 = """
                [
                    123,
                    "val",
                    {
                ]
                """;
            var json4 = """[]asdf""";
            var json5 = """[[]""";
            var json6 = """[[]]]""";
            var json7 = """[]]]""";
            var json8 = """[,]""";
            var json9 = """[[][]""";
            var json10 = """"["a":false]"""";
            var json11 = """"[["a":false],[]]"""";
            var json12 = """"["	tab	character	in	string	"]"""";
            var json13 = 
                        """"
                        ["line
                        break"]
                        """";

            Assert.Equal(1, parser.IsValid(json));
            Assert.Equal(1, parser.IsValid(json2));
            Assert.Equal(1, parser.IsValid(json3));
            Assert.Equal(1, parser.IsValid(json4));
            Assert.Equal(1, parser.IsValid(json5));
            Assert.Equal(1, parser.IsValid(json6));
            Assert.Equal(1, parser.IsValid(json7));
            Assert.Equal(1, parser.IsValid(json8));
            Assert.Equal(1, parser.IsValid(json9));
            Assert.Equal(1, parser.IsValid(json10));
            Assert.Equal(1, parser.IsValid(json11));
            Assert.Equal(1, parser.IsValid(json12));
            Assert.Equal(1, parser.IsValid(json13));
        }

        [Theory]
        [ClassData(typeof(JsonTestDataPass))]
        public void IsValid_ShouldPass(int index, string json, int expectedVal)
        {
            var parser = new JSONParser.JsonParser();
            Assert.Equal(expectedVal, parser.IsValid(json));
        }

        [Theory]
        [ClassData(typeof(JsonTestDataFail))]
        public void IsValid_ShouldFail(int index, string json, int expectedVal)
        {
            var parser = new JsonParser();
            var result = parser.IsValid(json);

            Assert.Equal(expectedVal, result);
        }
    }

    public class JsonTestDataPass : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var json = """"
                [
                    "JSON Test Pattern pass1",
                    {"object with 1 member":["array with 1 element"]},
                    {},
                    [],
                    -42,
                    true,
                    false,
                    null,
                    {
                        "integer": 1234567890,
                        "real": -9876.543210,
                        "e": 0.123456789e-12,
                        "E": 1.234567890E+34,
                        "":  23456789012E66,
                        "zero": 0,
                        "one": 1,
                        "space": " ",
                        "quote": "\"",
                        "backslash": "\\",
                        "controls": "\b\f\n\r\t",
                        "slash": "/ & \/",
                        "alpha": "abcdefghijklmnopqrstuvwyz",
                        "ALPHA": "ABCDEFGHIJKLMNOPQRSTUVWYZ",
                        "digit": "0123456789",
                        "0123456789": "digit",
                        "special": "`1~!@#$%^&*()_+-={':[,]}|;.</>?",
                        "hex": "\u0123\u4567\u89AB\uCDEF\uabcd\uef4A",
                        "true": true,
                        "false": false,
                        "null": null,
                        "array":[  ],
                        "object":{  },
                        "address": "50 St. James Street",
                        "url": "http://www.JSON.org/",
                        "comment": "// /* <!-- --",
                        "# -- --> */": " ",
                        " s p a c e d " :[1,2 , 3

                ,

                4 , 5        ,          6           ,7        ],"compact":[1,2,3,4,5,6,7],
                        "jsontext": "{\"object with 1 member\":[\"array with 1 element\"]}",
                        "quotes": "&#34; \u0022 %22 0x22 034 &#x22;",
                        "\/\\\"\uCAFE\uBABE\uAB98\uFCDE\ubcda\uef4A\b\f\n\r\t`1~!@#$%^&*()_+-=[]{}|;:',./<>?"
                : "A key can be any string"
                    },
                    0.5 ,98.6
                ,
                99.44
                ,

                1066,
                1e1,
                0.1e1,
                1e-1,
                1e00,2e+00,2e-00
                ,"rosebud"]
                """";

            yield return new object[] {1, json, 0 };            
            yield return new object[] {2, """[[[[[[[[[[[[[[[[[[["Not too deep"]]]]]]]]]]]]]]]]]]]""", 0};
            yield return new object[] {3, """
                {
                    "JSON Test Pattern pass3": {
                        "The outermost value": "must be an object or array.",
                        "In this test": "It is an object."
                    }
                }
                """, 0};
            
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class JsonTestDataFail : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 1, """"A JSON payload should be an object or array, not a string."""", 1 };
            yield return new object[] { 2, """"["Unclosed array" """", 1 };
            yield return new object[] { 3, """"{unquoted_key: "keys must be quoted"}"""", 1 };
            yield return new object[] { 4, """"["extra comma",]"""", 1 };
            yield return new object[] { 5, """"["double extra comma",,]"""", 1 };
            yield return new object[] { 6, """"[   , "<-- missing value"]"""", 1 };
            yield return new object[] { 7, """"["Comma after the close"],"""", 1 };
            yield return new object[] { 8, """"["Extra close"]]"""", 1 };
            yield return new object[] { 9, """"{"Extra comma": true,}"""", 1 };
            yield return new object[] { 10, """"
                                            {"Extra value after close": true} "misplaced quoted value"
                                            """", 1 };
            yield return new object[] { 11, """"{"Illegal expression": 1 + 2}"""", 1 };
            yield return new object[] { 12, """"{"Illegal invocation": alert()}"""", 1 };
            yield return new object[] { 13, """"{"Numbers cannot have leading zeroes": 013}"""", 1 };
            yield return new object[] { 14, """"{"Numbers cannot be hex": 0x14}"""", 1 };
            yield return new object[] { 15, """"["Illegal backslash escape: \x15"]"""", 1 };
            yield return new object[] { 16, """"[\naked]"""", 1 };
            yield return new object[] { 17, """"["Illegal backslash escape: \017"]"""", 1 };
            yield return new object[] { 18, """"[[[[[[[[[[[[[[[[[[[["Too deep"]]]]]]]]]]]]]]]]]]]]"""", 1 };//up to 20 layers of nesting? other validators don't have this validation
            yield return new object[] { 19, """"{"Missing colon" null}"""", 1 };
            yield return new object[] { 20, """"{"Double colon":: null}"""", 1 };
            yield return new object[] { 21, """"{"Comma instead of colon", null}"""", 1 };
            yield return new object[] { 22, """"["Colon instead of comma": false]"""", 1 };
            yield return new object[] { 23, """"["Bad value", truth]"""", 1 };
            yield return new object[] { 24, """"['single quote']"""", 1 };
            yield return new object[] { 25, """"["	tab	character	in	string	"]"""", 1 };
            yield return new object[] { 26, """"["tab\   character\   in\  string\  "]"""", 1 };
            yield return new object[] { 27, """"
                                            ["line
                                            break"]
                                            """", 1 };
            yield return new object[] { 28, """"
                                            ["line\
                                            break"]
                                            """", 1 };
            yield return new object[] { 29, """"[0e]"""", 1 };
            yield return new object[] { 30, """"[0e+]"""", 1 };
            yield return new object[] { 31, """"[0e+-1]"""", 1 };
            yield return new object[] { 32, """"{"Comma instead if closing brace": true,"""", 1 };
            yield return new object[] { 33, """"["mismatch"}"""", 1 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
