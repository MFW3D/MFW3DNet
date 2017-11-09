//----------------------------------------------------------------------------------------------------------------------------
// NAME: GPSTracker
// DEVELOPER: Javier Santoro
// WEBSITE: http://www.worldwindcentral.com/wiki/Add-on:GPS_Tracker_(plugin)
// VERSION: V04R06 (May 16, 2007)
// COPYRIGHT NOTICE: Copyright 2007, Javier Santoro
//                   Permission is granted to use GpsTracker for non-commercial purposes. 
//                   Permission is granted to use GpsTracker source code for educational and non-commercial purposes.
//----------------------------------------------------------------------------------------------------------------------------

using System.Drawing;

namespace GpsTracker
{
    //
    // This class holds all the necessary information for rendering a GPS source

	public class GPSRenderInformation
	{
		public int			iStartAltitud;
		public bool			bPOI;
		public int			iIndex;
		public int			iActiveTrack;
		public string		sDescription;
		public bool			fFix;
		public double		fLat;
		public double		fLon;
		public float		fAlt;
		public float		fESpeed;
		public float		fNSpeed;
		public float		fVSpeed;
		public float		fRoll;
		public float		fPitch;
		public float		fDepth;
		public string		sAltUnit;
		public string		sSpeedUnit;
		public float		fSpeed;
		public float		fHeading;
		public string		sIcon;
		public string		sPortInfo;
		public int			iHour;
		public int			iMin;
		public float		fSec;
		public bool			bShowInfo;
		public bool			bTrackLine;
		public GPSTrack		gpsTrack;
		public bool			bRestartTrack;
		public int			iDay;
		public int			iMonth;
		public int			iYear;
		public Color		colorTrack;
		public string		sComment;
		public int			iAPRSIconTable;
		public int			iAPRSIconCode;
		public bool			fTrack;
        public bool         fTrackOnTop;
        public bool         bShowGrid;
        public int          iSquareCount;
        public double       dSquareSize;
        public bool         bGridKms;
        public Color        colorGrid;
        public bool         bShowCircles;
        public int          iCirclesCount;
        public double       dCirclesEvery;
        public int          iLinesEvery;
        public Color        colorCircle;
        public bool         bKms;
        public bool         bShowName;
        public bool         bShowPosition;
        public bool         bShowSpeed;
        public bool         bShowHeading;
        public bool         bShowTimeDate;
        public bool         bShowTrackDistance;
        public bool         bShowComment;
        public bool         bShowReferenceDistance;
        public bool         bShowReferenceAngles;
        public int          iInformationFontSize;
        public Color        colorInformation;
        public int iPositionUnit;
        public int iDistanceUnit;
        public int iSpeedUnit;
        public int iAltitudeUnit;
        public int iTotalLines;
        public int iCurrentLine;

		public GPSRenderInformation()
		{
			Init();
		}

		public void Init()
		{
			bPOI=false;
			iIndex=0;
			sDescription="";
			sComment="";
			fFix=false;
			fLat=0F;
			fLon=0F;
			fAlt=0F;
			fESpeed=-1000000F;
			fNSpeed=-1000000F;
			fVSpeed=-1000000F;
			fRoll=-1000F;
			fPitch=-1000F;
			sAltUnit="";
			sSpeedUnit="";
			fSpeed=0F;
			fHeading=0F;
			sIcon="";
			sPortInfo="";
			iHour=0;
			fSec=0F;
			iMonth=0;
			iYear=0;
			bShowInfo=false;
			bTrackLine=false;
			gpsTrack=null;
			bRestartTrack=false;
			colorTrack=Color.FromArgb(255);
			iAPRSIconTable=-1;
			iAPRSIconCode=-1;
			fTrack=false;
            bShowName=true;
            bShowPosition = true;
            bShowSpeed = true;
            bShowHeading = true;
            bShowTimeDate = true;
            bShowTrackDistance = true;
            bShowComment = true;
            bShowReferenceDistance = true;
            bShowReferenceAngles = true;
            iInformationFontSize = 0;
            colorInformation = Color.Yellow;
            bShowGrid = false;
            

		}
	}



}


