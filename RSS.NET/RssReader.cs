using System;
using System.Collections;
using System.Xml;
using System.Text;
using System.IO;

namespace Rss
{
	/// <summary>Reads an RSS file.</summary>
	/// <remarks>Provides fast, non-cached, forward-only access to RSS data.</remarks>
	public class RssReader
	{
		// TODO: Add support for modules

		private Stack xmlNodeStack = new Stack();
		private StringBuilder elementText = new StringBuilder();
		private XmlTextReader reader = null;
		private bool wroteChannel = false;
		private RssVersion rssVersion = RssVersion.Empty;
		private ExceptionCollection exceptions = new ExceptionCollection();

		private RssTextInput textInput = null;
		private RssImage image = null;
		private RssCloud cloud = null;
		private RssChannel channel = null;
		private RssSource source = null;
		private RssEnclosure enclosure = null;
		private RssGuid guid = null;
		private RssCategory category = null;
		private RssItem item = null;

		private void InitReader()
		{
			reader.WhitespaceHandling = WhitespaceHandling.None;
			reader.XmlResolver = null;
		}

		#region Constructors

		/// <summary>Initializes a new instance of the RssReader class with the specified URL or filename.</summary>
		/// <param name="url">The URL or filename for the file containing the RSS data.</param>
		/// <exception cref="ArgumentException">Occures when unable to retrieve file containing the RSS data.</exception>
		public RssReader(string url)
		{
			try
			{
				reader = new XmlTextReader(url);
				InitReader();
			}
			catch (Exception e)
			{
				throw new ArgumentException("Unable to retrieve file containing the RSS data.", e);
			}
		}

		/// <summary>Creates an instance of the RssReader class using the specified TextReader.</summary>
		/// <param name="textReader">specified TextReader</param>
		/// <exception cref="ArgumentException">Occures when unable to retrieve file containing the RSS data.</exception>
		public RssReader(TextReader textReader)
		{
			try
			{
				reader = new XmlTextReader(textReader);
				InitReader();
			}
			catch (Exception e)
			{
				throw new ArgumentException("Unable to retrieve file containing the RSS data.", e);
			}
		}

		/// <summary>Creates an instance of the RssReader class using the specified Stream.</summary>
		/// <exception cref="ArgumentException">Occures when unable to retrieve file containing the RSS data.</exception>
		/// <param name="stream">Stream to read from</param>
		public RssReader(Stream stream)
		{
			try
			{
				reader = new XmlTextReader(stream);
				InitReader();
			}
			catch (Exception e)
			{
				throw new ArgumentException("Unable to retrieve file containing the RSS data.", e);
			}
		}

		#endregion

		/// <summary>Reads the next RssElement from the stream.</summary>
		/// <returns>An RSS Element</returns>
		/// <exception cref="InvalidOperationException">RssReader has been closed, and can not be read.</exception>
		/// <exception cref="System.IO.FileNotFoundException">RSS file not found.</exception>
		/// <exception cref="System.Xml.XmlException">Invalid XML syntax in RSS file.</exception>
		/// <exception cref="System.IO.EndOfStreamException">Unable to read an RssElement. Reached the end of the stream.</exception>
		public RssElement Read()
		{
			bool readData = false;
			bool pushElement = true;
			RssElement rssElement = null;
			int lineNumber = -1;
			int linePosition = -1;

			if (reader == null)
				throw new InvalidOperationException("RssReader has been closed, and can not be read.");

			do
			{
				pushElement = true;
				try
				{
					readData = reader.Read();
				}
				catch (System.IO.EndOfStreamException e)
				{
					throw new System.IO.EndOfStreamException("Unable to read an RssElement. Reached the end of the stream.", e);
				}
				catch (System.Xml.XmlException e)
				{
					if (lineNumber != -1 || linePosition != -1)
						if (reader.LineNumber == lineNumber && reader.LinePosition == linePosition)
							throw exceptions.LastException;

					lineNumber = reader.LineNumber;
					linePosition = reader.LinePosition;

					exceptions.Add(e); // just add to list of exceptions and continue :)
				}
				if (readData)
				{
					string readerName = reader.Name.ToLower();
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
						{
							if (reader.IsEmptyElement)
								break;
							elementText = new StringBuilder();

							switch (readerName)
							{
								case "item":
									// is this the end of the channel element? (absence of </channel> before <item>)
									if (!wroteChannel)
									{
										wroteChannel = true;
										rssElement = channel; // return RssChannel
										readData = false;
									}
									item = new RssItem(); // create new RssItem
									channel.Items.Add(item);
									break;
								case "source":
									source = new RssSource();
									item.Source = source;
									for (int i=0; i < reader.AttributeCount; i++)
									{
										reader.MoveToAttribute(i);
										switch (reader.Name.ToLower())
										{
											case "url":
												try
												{
													source.Url = new Uri(reader.Value);
												}				
												catch (Exception e)
												{
													exceptions.Add(e);
												}
												break;
										}
									}
									break;
								case "enclosure":
									enclosure = new RssEnclosure();
									item.Enclosure = enclosure;
									for (int i=0; i < reader.AttributeCount; i++)
									{
										reader.MoveToAttribute(i);
										switch (reader.Name.ToLower())
										{
											case "url":
												try
												{
													enclosure.Url = new Uri(reader.Value);
												}				
												catch (Exception e)
												{
													exceptions.Add(e);
												}
												break;
											case "length":
												try
												{
													enclosure.Length = int.Parse(reader.Value);
												}				
												catch (Exception e)
												{
													exceptions.Add(e);
												}
												break;
											case "type":
												enclosure.Type = reader.Value;
												break;
										}
									}
									break;
								case "guid":
									guid = new RssGuid();
									item.Guid = guid;
									for (int i=0; i < reader.AttributeCount; i++)
									{
										reader.MoveToAttribute(i);
										switch (reader.Name.ToLower())
										{
											case "ispermalink":
												try
												{
													guid.PermaLink = bool.Parse(reader.Value);
												}			
												catch (Exception e)
												{
													exceptions.Add(e);
												}
												break;
										}
									}
									break;
								case "category":
									category = new RssCategory();
									if ((string)xmlNodeStack.Peek() == "channel")
										channel.Categories.Add(category);
									else
										item.Categories.Add(category);
									for (int i=0; i < reader.AttributeCount; i++)
									{
										reader.MoveToAttribute(i);
										switch (reader.Name.ToLower())
										{
											case "url":
												goto case "domain";
											case "domain":
												category.Domain = reader.Value;
												break;
										}
									}
									break;
								case "channel":
									channel = new RssChannel();
									textInput = null;
									image = null;
									cloud = null;
									source = null;
									enclosure = null;
									category = null;
									item = null;
									break;
								case "image":
									image = new RssImage();
									channel.Image = image;
									break;
								case "textinput":
									textInput = new RssTextInput();
									channel.TextInput = textInput;
									break;
								case "cloud":
									pushElement = false;
									cloud = new RssCloud();
									channel.Cloud = cloud;
									for (int i=0; i < reader.AttributeCount; i++)
									{
										reader.MoveToAttribute(i);
										switch (reader.Name.ToLower())
										{
											case "domain":
												cloud.Domain = reader.Value;
												break;
											case "port":
												try
												{
													cloud.Port = ushort.Parse(reader.Value);
												}				
												catch (Exception e)
												{
													exceptions.Add(e);
												}
												break;
											case "path":
												cloud.Path = reader.Value;
												break;
											case "registerprocedure":
												cloud.RegisterProcedure = reader.Value;
												break;
											case "protocol":
											switch (reader.Value.ToLower())
											{
												case "xml-rpc":
													cloud.Protocol = RssCloudProtocol.XmlRpc;
													break;
												case "soap":
													cloud.Protocol = RssCloudProtocol.Soap;
													break;
												case "http-post":
													cloud.Protocol = RssCloudProtocol.HttpPost;
													break;
												default:
													cloud.Protocol = RssCloudProtocol.Empty;
													break;
											}
												break;
										}
									}
									break;
								case "rss":
									for (int i=0; i < reader.AttributeCount; i++)
									{
										reader.MoveToAttribute(i);
										if (reader.Name.ToLower() == "version")
											switch (reader.Value)
											{
												case "0.91":
													rssVersion = RssVersion.RSS091;
													break;
												case "0.92":
													rssVersion = RssVersion.RSS092;
													break;
												case "2.0":
													rssVersion = RssVersion.RSS20;
													break;
												default:
													rssVersion = RssVersion.NotSupported;
													break;
											}
									}
									break;
								case "rdf":
									for (int i=0; i < reader.AttributeCount; i++)
									{
										reader.MoveToAttribute(i);
										if (reader.Name.ToLower() == "version")
											switch (reader.Value)
											{
												case "0.90":
													rssVersion = RssVersion.RSS090;
													break;
												case "1.0":
													rssVersion = RssVersion.RSS10;
													break;
												default:
													rssVersion = RssVersion.NotSupported;
													break;
											}
									}
									break;
                                case "geo:lat":
                                case "geo:long":
                                case "geo:point":
                                case "georss:point":
                                case "georss:elev":
                                case "georss:radius":
                                    if (item.GeoPoint == null)
                                        item.GeoPoint = new RssGeoPoint();
                                    break;
							}
							if (pushElement)
								xmlNodeStack.Push(readerName);
							break;
						}
						case XmlNodeType.EndElement:
						{
							if (xmlNodeStack.Count == 1)
								break;
							string childElementName = (string)xmlNodeStack.Pop();
							string parentElementName = (string)xmlNodeStack.Peek();
							switch (childElementName) // current element
							{
									// item classes
								case "item":
									rssElement = item;
									readData = false;
									break;
								case "source":
									source.Name = elementText.ToString();
									rssElement = source;
									readData = false;
									break;
								case "enclosure":
									rssElement = enclosure;
									readData = false;
									break;
								case "guid":
									guid.Name = elementText.ToString();
									rssElement = guid;
									readData = false;
									break;
								case "category": // parent is either item or channel
									category.Name = elementText.ToString();
									rssElement = category;
									readData = false;
									break;
									// channel classes
								case "channel":
									if (wroteChannel)
										wroteChannel = false;
									else
									{
										wroteChannel = true;
										rssElement = channel;
										readData = false;
									}
									break;
								case "textinput":
									rssElement = textInput;
									readData = false;
									break;
								case "image":
									rssElement = image;
									readData = false;
									break;
								case "cloud":
									rssElement = cloud;
									readData = false;
									break;
							}
							switch (parentElementName) // parent element
							{
								case "item":
								switch (childElementName)
								{
									case "title":
										item.Title = elementText.ToString();
										break;
									case "link":
										item.Link = new Uri(elementText.ToString());
										break;
									case "description":
										item.Description = elementText.ToString();
										break;
									case "author":
										item.Author = elementText.ToString();
										break;
									case "comments":
										item.Comments = elementText.ToString();
										break;
									case "pubdate":
										try
										{
											item.PubDate = DateTime.Parse(elementText.ToString());
										}				
										catch (Exception e)
										{
											try {
											string tmp = elementText.ToString ();
											tmp = tmp.Substring (0, tmp.Length - 5);
											tmp += "GMT";
											item.PubDate = DateTime.Parse (tmp);
											}
											catch 
											{
											exceptions.Add(e);
											}
										}
										break;
                                    case "geo:lat":
                                        try
                                        {
                                            item.GeoPoint.Lat = float.Parse(elementText.ToString());

                                        }
                                        catch (Exception e)
                                        {
                                            exceptions.Add(e);
                                        }
                                        break;
                                    case "geo:long":
                                        try
                                        {
                                            item.GeoPoint.Lon = float.Parse(elementText.ToString());
                                        }
                                        catch (Exception e)
                                        {
                                            exceptions.Add(e);
                                        }
                                        break;
                                    case "georss:point":
                                        try
                                        {
                                            string[] coords = elementText.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                            if (coords.Length == 2)
                                            {
                                                item.GeoPoint.Lat = float.Parse(coords[0]);
                                                item.GeoPoint.Lon = float.Parse(coords[1]);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            exceptions.Add(e);
                                        }
                                        break;
								}
									break;
								case "channel":
								switch (childElementName)
								{
									case "title":
										channel.Title = elementText.ToString();
										break;
									case "link":
										try
										{
											channel.Link = new Uri(elementText.ToString());
										}				
										catch (Exception e)
										{
											exceptions.Add(e);
										}
										break;
									case "description":
										channel.Description = elementText.ToString();
										break;
									case "language":
										channel.Language = elementText.ToString();
										break;
									case "copyright":
										channel.Copyright = elementText.ToString();
										break;
									case "managingeditor":
										channel.ManagingEditor = elementText.ToString();
										break;
									case "webmaster":
										channel.WebMaster = elementText.ToString();
										break;
									case "rating":
										channel.Rating = elementText.ToString();
										break;
									case "pubdate":
										try
										{
											channel.PubDate = DateTime.Parse(elementText.ToString());
										}				
										catch (Exception e)
										{
											exceptions.Add(e);
										}
										break;
									case "lastbuilddate":
										try
										{
											channel.LastBuildDate = DateTime.Parse(elementText.ToString());
										}				
										catch (Exception e)
										{
											exceptions.Add(e);
										}
										break;
									case "generator":
										channel.Generator = elementText.ToString();
										break;
									case "docs":
										channel.Docs = elementText.ToString();
										break;
									case "ttl":
										try
										{
											channel.TimeToLive = int.Parse(elementText.ToString());
										}				
										catch (Exception e)
										{
											exceptions.Add(e);
										}
										break;
								}
									break;
								case "image":
								switch (childElementName)
								{
									case "url":
										try
										{
											image.Url = new Uri(elementText.ToString());
										}				
										catch (Exception e)
										{
											exceptions.Add(e);
										}
										break;
									case "title":
										image.Title = elementText.ToString();
										break;
									case "link":
										try
										{
											image.Link = new Uri(elementText.ToString());
										}				
										catch (Exception e)
										{
											exceptions.Add(e);
										}
										break;
									case "description":
										image.Description = elementText.ToString();
										break;
									case "width":
										try
										{
											image.Width = Byte.Parse(elementText.ToString());
										}				
										catch (Exception e)
										{
											exceptions.Add(e);
										}
										break;
									case "height":
										try
										{
											image.Height = Byte.Parse(elementText.ToString());
										}				
										catch (Exception e)
										{
											exceptions.Add(e);
										}
										break;
								}
									break;
								case "textinput":
								switch (childElementName)
								{
									case "title":
										textInput.Title = elementText.ToString();
										break;
									case "description":
										textInput.Description = elementText.ToString();
										break;
									case "name":
										textInput.Name = elementText.ToString();
										break;
									case "link":
										try
										{
											textInput.Link = new Uri(elementText.ToString());
										}				
										catch (Exception e)
										{
											exceptions.Add(e);
										}
										break;
								}
									break;
								case "skipdays":
									if (childElementName == "day")
                                        channel.SkipDays |= Day.Parse(elementText.ToString());
									break;
								case "skiphours":
									if (childElementName == "hour")
										channel.SkipHours |= Hour.Parse(elementText.ToString());
									break;
							}
							break;
						}
						case XmlNodeType.Text:
							elementText.Append(reader.Value);
							break;
						case XmlNodeType.CDATA:
							elementText.Append(reader.Value);
							break;
					}
				}
			}
			while (readData);
			return rssElement;
		}
		/// <summary>A collection of all exceptions the RssReader class has encountered.</summary>
		public ExceptionCollection Exceptions
		{
			get { return exceptions; }
		}
		/// <summary>Gets the RSS version of the stream.</summary>
		/// <value>One of the <see cref="RssVersion"/> values.</value>
		public RssVersion Version
		{
			get { return rssVersion; }
		}
		/// <summary>Closes connection to file.</summary>
		/// <remarks>This method also releases any resources held while reading.</remarks>
		public void Close()
		{
			textInput = null;
			image = null;
			cloud = null;
			channel = null;
			source = null;
			enclosure = null;
			category = null;
			item = null;
			if (reader!=null)
			{
				reader.Close();
				reader = null;
			}
			elementText = null;
			xmlNodeStack = null;
		}
	}
}
