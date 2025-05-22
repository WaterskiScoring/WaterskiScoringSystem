//------------------------------------------------------------
## v19.07
ALTER TABLE [SlalomRecap] ADD COLUMN  PassSpeedKph tinyint;
ALTER TABLE [SlalomRecap] DROP COLUMN PassNum;
ALTER TABLE [SlalomScore] DROP COLUMN FinalPassNum;

Update [SlalomRecap] Set PassSpeedKph = SUBSTRING ( Note, CHARINDEX('kph', Note) - 2, 2 );

## v19.08
ALTER TABLE EventRunOrder ADD COLUMN  RunOrderGroup nvarchar(12);

Update EventRunOrder Set RunOrderGroup = '';

## v20.08
DROP TABLE EventRunOrderFilters;

CREATE TABLE EventRunOrderFilters (
    SanctionId nchar(6) NOT NULL
    , Event nvarchar(12) NOT NULL
    , FilterName nvarchar(128) NOT NULL
    , PrintTitle nvarchar(256)
    , GroupFilterCriteria nvarchar(1024) NOT NULL
    , LastUpdateDate datetime 
);

## v21.11
CREATE TABLE EwscMsg (
    PK          int NOT NULL IDENTITY,
    SanctionId  nchar(6) NOT NULL,
    MsgType     nvarchar(128) NOT NULL,
    MsgData     nvarchar(3584) NOT NULL,
    CreateDate  datetime
);

ALTER TABLE [EwscMsg] ADD PRIMARY KEY ([PK]);

## v21.13
CREATE TABLE EwscListenMsg (
    PK          int NOT NULL IDENTITY,
    SanctionId  nchar(6) NOT NULL,
    MsgType     nvarchar(128) NOT NULL,
    MsgData     nvarchar(3584) NOT NULL,
    CreateDate  datetime
);

ALTER TABLE [EwscListenMsg] ADD PRIMARY KEY ([PK]);


## v22.09
ALTER TABLE [SlalomScore] ADD COLUMN InsertDate datetime;
ALTER TABLE [SlalomRecap] ADD COLUMN InsertDate datetime;
ALTER TABLE [JumpScore] ADD COLUMN InsertDate datetime;
ALTER TABLE [JumpRecap] ADD COLUMN InsertDate datetime;
ALTER TABLE [TrickScore] ADD COLUMN InsertDate datetime;

Update [SlalomScore] Set InsertDate = LastUpdateDate Where InsertDate is null;
Update [SlalomRecap] Set InsertDate = LastUpdateDate Where InsertDate is null;
Update [JumpScore] Set InsertDate = LastUpdateDate Where InsertDate is null;
Update [JumpRecap] Set InsertDate = LastUpdateDate Where InsertDate is null;
Update [TrickScore] Set InsertDate = LastUpdateDate Where InsertDate is null;

DROP TABLE [BoatTime];
DROP TABLE [BoatPath];
DROP TABLE [JumpMeasurement];

CREATE TABLE [BoatTime] (
    PK bigint NOT NULL IDENTITY
	, SanctionId nchar(6) NOT NULL
	, MemberId nchar(9) NOT NULL
    , Event nvarchar(12) NOT NULL

	, Round tinyint NOT NULL
	, PassNumber tinyint NOT NULL

	, PassLineLength numeric(5,2) NULL
	, PassSpeedKph tinyint

	, BoatTimeBuoy1 numeric(5,2) NULL
	, BoatTimeBuoy2 numeric(5,2) NULL
	, BoatTimeBuoy3 numeric(5,2) NULL
	, BoatTimeBuoy4 numeric(5,2) NULL
	, BoatTimeBuoy5 numeric(5,2) NULL
	, BoatTimeBuoy6 numeric(5,2) NULL
	, BoatTimeBuoy7 numeric(5,2) NULL

	, InsertDate datetime NULL
	, LastUpdateDate datetime NULL
);

ALTER TABLE [BoatTime] ADD PRIMARY KEY ([PK]);

CREATE TABLE [BoatPath] (
    PK bigint NOT NULL IDENTITY
	, SanctionId nchar(6) NOT NULL
	, MemberId nchar(9) NOT NULL
    , Event nvarchar(12) NOT NULL

	, Round tinyint NOT NULL
	, PassNumber tinyint NOT NULL

	, PassLineLength numeric(5,2) NULL
	, PassSpeedKph tinyint

	, PathDevBuoy0 numeric(5,2) NULL
	, PathDevBuoy1 numeric(5,2) NULL
	, PathDevBuoy2 numeric(5,2) NULL
	, PathDevBuoy3 numeric(5,2) NULL
	, PathDevBuoy4 numeric(5,2) NULL
	, PathDevBuoy5 numeric(5,2) NULL
	, PathDevBuoy6 numeric(5,2) NULL

	, PathDevCum0 numeric(5,2) NULL
	, PathDevCum1 numeric(5,2) NULL
	, PathDevCum2 numeric(5,2) NULL
	, PathDevCum3 numeric(5,2) NULL
	, PathDevCum4 numeric(5,2) NULL
	, PathDevCum5 numeric(5,2) NULL
	, PathDevCum6 numeric(5,2) NULL


	, PathDevZone0 numeric(5,2) NULL
	, PathDevZone1 numeric(5,2) NULL
	, PathDevZone2 numeric(5,2) NULL
	, PathDevZone3 numeric(5,2) NULL
	, PathDevZone4 numeric(5,2) NULL
	, PathDevZone5 numeric(5,2) NULL
	, PathDevZone6 numeric(5,2) NULL

    , RerideNote nvarchar(64)
	
	, InsertDate datetime NULL
	, LastUpdateDate datetime NULL
);

ALTER TABLE [BoatPath] ADD PRIMARY KEY ([PK]);

CREATE TABLE [JumpMeasurement] (
    PK bigint NOT NULL IDENTITY
	, SanctionId nchar(6) NOT NULL
	, MemberId nchar(9) NOT NULL
    , Event nvarchar(12) NOT NULL

	, Round tinyint NOT NULL
	, PassNumber tinyint NOT NULL

	, ScoreFeet numeric(5,2) NULL
	, ScoreMeters numeric(5,2) NULL

	, InsertDate datetime NULL
	, LastUpdateDate datetime NULL
);

ALTER TABLE [JumpMeasurement] ADD PRIMARY KEY ([PK]);

## v22.11
ALTER TABLE [BoatPath] ADD COLUMN DriverMemberId nchar(9) NULL;
ALTER TABLE [BoatPath] ADD COLUMN DriverName nchar(64) NULL;
ALTER TABLE [BoatPath] ADD COLUMN BoatDescription nchar(256) NULL;

## v22.14
ALTER TABLE [JumpRecap] ADD COLUMN SkierBoatPath nchar(6) NULL;
Update [JumpRecap] Set SkierBoatPath = 'S';

## v22.20
ALTER TABLE [EwscMsg] ALTER COLUMN MsgData ntext ;
DROP TABLE EwscListenMsg;

## v22.29
ALTER TABLE [BoatTime] ALTER COLUMN InsertDate datetime NOT NULL;
ALTER TABLE [BoatPath] ALTER COLUMN InsertDate datetime NOT NULL;

## v22.31
ALTER TABLE [JumpMeasurement] ALTER COLUMN InsertDate datetime NOT NULL;
## v22.34
ALTER TABLE EventRunOrderFilters ALTER COLUMN GroupFilterCriteria nvarchar(1024) NULL;

## v22.50
DROP TABLE EwscMsg;

CREATE TABLE WscMsgListen (
    PK          int NOT NULL IDENTITY,
    SanctionId  nchar(6) NOT NULL,
    MsgType     nvarchar(128) NOT NULL,
    MsgData     ntext NOT NULL,
    CreateDate  datetime
);

ALTER TABLE WscMsgListen ADD PRIMARY KEY ([PK]);


CREATE TABLE WscMsgSend (
    PK          int NOT NULL IDENTITY,
    SanctionId  nchar(6) NOT NULL,
    MsgType     nvarchar(128) NOT NULL,
    MsgData     ntext NOT NULL,
    CreateDate  datetime
);

ALTER TABLE WscMsgSend ADD PRIMARY KEY ([PK]);

## v22.51
CREATE TABLE WscMonitor (
    SanctionId  nchar(6) NOT NULL,
    MonitorName nvarchar(32) NOT NULL,
    HeartBeat  datetime
);

## v22.53
ALTER TABLE [TourReg] ADD COLUMN Withdrawn nchar(1);
Update TourReg Set Withdrawn = 'N' Where Withdrawn is null;

## v22.59
ALTER TABLE [MemberList] ALTER COLUMN MemberStatus nvarchar(256);

## v22.70
ALTER TABLE [BoatPath] ADD COLUMN homologation nchar(1);
Update [BoatPath] Set homologation = 'L' Where homologation is null;

## v22.71
ALTER TABLE [TourReg] ADD COLUMN IwwfLicense nchar(1);

Update [TourReg] Set IwwfLicense = 'N';

Update [TourReg] Set IwwfLicense = 'Y'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = TourReg.SanctionId AND R.MemberId = TourReg.MemberId AND R.EventClass in ('L', 'R'));

## v22.77
CREATE TABLE LiveWebMsgSend (
    PK          int NOT NULL IDENTITY,
    SanctionId  nchar(6) NOT NULL,
    MsgType     nvarchar(128) NOT NULL,
    MsgDataHash int NOT NULL,
    MsgData     ntext NOT NULL,
    CreateDate  datetime
);

ALTER TABLE LiveWebMsgSend ADD PRIMARY KEY ([PK]);

## v22.81
ALTER TABLE [TourReg] ADD COLUMN SlalomClassReg nvarchar(16);
ALTER TABLE [TourReg] ADD COLUMN TrickClassReg nvarchar(16);
ALTER TABLE [TourReg] ADD COLUMN JumpClassReg nvarchar(16);

## v22.82
ALTER TABLE [TourReg] ADD COLUMN Team nvarchar(16);

## v22.83
ALTER TABLE [SlalomRecap] ADD COLUMN ProtectedScore numeric(5,2);
ALTER TABLE [SlalomRecap] ADD COLUMN BoatPathGood nchar(1);

## v22.86
ALTER TABLE [BoatTime] ALTER COLUMN InsertDate datetime NULL;
ALTER TABLE [BoatPath] ALTER COLUMN InsertDate datetime NULL;
ALTER TABLE [JumpMeasurement] ALTER COLUMN InsertDate datetime NULL;

CREATE TABLE WscMonitorMsg (
    PK          int NOT NULL IDENTITY,
    SanctionId  nchar(6) NOT NULL,
    MsgAction   nvarchar(16) NOT NULL,
    MsgType     nvarchar(128) NOT NULL,
    MsgData     nvarchar(128) NOT NULL,
    InsertDate  datetime
);

ALTER TABLE WscMonitorMsg ADD PRIMARY KEY ([PK]);

## v22.90
ALTER TABLE [JumpRecap] ADD COLUMN  RerideIfBest nchar(1);
ALTER TABLE [JumpRecap] ADD COLUMN  RerideCanImprove nchar(1);

//------------------------------------------------------------
## v23.08
ALTER TABLE MemberList ADD COLUMN ForeignFederationID nvarchar(12);

ALTER TABLE TourReg ADD COLUMN ForeignFederationID nvarchar(12);

## v23.10
ALTER TABLE [TourBoatUse] ALTER COLUMN HullId nvarchar(16) NOT NULL;


//------------------------------------------------------------
## v24.10
CREATE TABLE TeamScore (
    SanctionId nchar(6) NOT NULL
    , TeamCode nvarchar(12) NOT NULL
    , AgeGroup nvarchar(12) NOT NULL
    , Name nvarchar(64) NOT NULL
    , ReportFormat nvarchar(16) NOT NULL
    , OverallPlcmt nvarchar(8) DEFAULT NULL
    , SlalomPlcmt nvarchar(8) DEFAULT NULL
    , TrickPlcmt nvarchar(8) DEFAULT NULL
    , JumpPlcmt nvarchar(8) DEFAULT NULL
    , OverallScore numeric(7,2) DEFAULT NULL
    , SlalomScore numeric(7,2) DEFAULT NULL
    , TrickScore numeric(7,2) DEFAULT NULL
    , JumpScore numeric(7,2) DEFAULT NULL
    , LastUpdateDate datetime NULL
);
ALTER TABLE TeamScore ADD PRIMARY KEY (SanctionId, TeamCode, AgeGroup) ;

CREATE TABLE TeamScoreDetail (
    SanctionId nchar(6) NOT NULL
    , TeamCode nvarchar(12) NOT NULL
    , AgeGroup nvarchar(12) NOT NULL
    , LineNum smallint NOT NULL
    , SkierCategory nvarchar(12) DEFAULT NULL
    , SlalomSkierName nvarchar(64) DEFAULT NULL
    , SlalomPlcmt nvarchar(8) DEFAULT NULL
    , SlalomScore numeric(7,2) DEFAULT NULL
    , SlalomNops numeric(7,2) DEFAULT NULL
    , SlalomPoints numeric(7,2) DEFAULT NULL
    , TrickSkierName nvarchar(64) DEFAULT NULL
    , TrickPlcmt nvarchar(8) DEFAULT NULL
    , TrickScore int DEFAULT NULL
    , TrickNops numeric(7,2) DEFAULT NULL
    , TrickPoints numeric(7,2) DEFAULT NULL
    , JumpSkierName nvarchar(64) DEFAULT NULL
    , JumpPlcmt nvarchar(8) DEFAULT NULL
    , JumpScore numeric(7,2) DEFAULT NULL
    , JumpNops numeric(7,2) DEFAULT NULL
    , JumpPoints numeric(7,2) DEFAULT NULL
);
ALTER TABLE TeamScoreDetail ADD PRIMARY KEY (SanctionId, TeamCode, AgeGroup, LineNum) ;


//------------------------------------------------------------
## v24.14
ALTER TABLE TrickVideo ALTER COLUMN Pass1VideoUrl nvarchar(512) NULL;
ALTER TABLE TrickVideo ALTER COLUMN Pass2VideoUrl nvarchar(512) NULL;

//------------------------------------------------------------
## v24.15
DROP TABLE LiveWebStatus;

CREATE TABLE LiveWebStatus (
    SanctionId  nchar(6) NOT NULL,
    StatusActive nchar(1) NOT NULL,
    LastUpdateDate datetime
);

ALTER TABLE LiveWebStatus ADD PRIMARY KEY (SanctionId);

ALTER TABLE Tournament DROP COLUMN PlcmtMethod;
ALTER TABLE Tournament DROP COLUMN OverallMethod;

## v24.17
ALTER TABLE [OfficialWorkAsgmt] ALTER COLUMN StartTime datetime NULL;

## v24.19
ALTER TABLE [Tournament] ADD COLUMN PlcmtMethod nvarchar(256);
ALTER TABLE [Tournament] ADD COLUMN OverallMethod nvarchar(256);

DROP TABLE EventRunOrderFiltersBackup;
CREATE TABLE EventRunOrderFiltersBackup (
    SanctionId nchar(6) NOT NULL
    , Event nvarchar(12) NOT NULL
    , FilterName nvarchar(128) NOT NULL
    , PrintTitle nvarchar(256)
    , GroupFilterCriteria nvarchar(1024) NULL
    , LastUpdateDate datetime 
);
Insert into EventRunOrderFiltersBackup (
    SanctionId, Event, FilterName, PrintTitle, GroupFilterCriteria, LastUpdateDate
) 
Select SanctionId, Event, FilterName, PrintTitle, GroupFilterCriteria, LastUpdateDate From EventRunOrderFilters;

DROP TABLE EventRunOrderFilters;
CREATE TABLE EventRunOrderFilters (
    SanctionId nchar(6) NOT NULL
    , Event nvarchar(12) NOT NULL
    , FilterName nvarchar(128) NOT NULL
    , PrintTitle nvarchar(256)
    , GroupFilterCriteria nvarchar(1024) NULL
    , LastUpdateDate datetime 
);
Insert into EventRunOrderFilters (
    SanctionId, Event, FilterName, PrintTitle, GroupFilterCriteria, LastUpdateDate
) 
Select SanctionId, Event, FilterName, PrintTitle, GroupFilterCriteria, LastUpdateDate From EventRunOrderFiltersBackup ;
DROP TABLE EventRunOrderFiltersBackup;


## v25.02
ALTER TABLE [MemberList] ADD COLUMN TechControllerSlalomRating nvarchar(32);
ALTER TABLE [MemberList] ADD COLUMN TechControllerTrickRating nvarchar(32);
ALTER TABLE [MemberList] ADD COLUMN TechControllerJumpRating nvarchar(32);

ALTER TABLE OfficialWork ADD COLUMN TechControllerSlalomRating nvarchar(32);
ALTER TABLE OfficialWork ADD COLUMN TechControllerTrickRating nvarchar(32);
ALTER TABLE OfficialWork ADD COLUMN TechControllerJumpRating nvarchar(32);

Update MemberList
SET TechControllerSlalomRating = TechOfficialRating
, TechControllerTrickRating = TechOfficialRating
, TechControllerJumpRating = TechOfficialRating

Update OfficialWork
SET TechControllerSlalomRating = TechOfficialRating
, TechControllerTrickRating = TechOfficialRating
, TechControllerJumpRating = TechOfficialRating


ALTER TABLE [MemberList] DROP COLUMN TechOfficialRating;
ALTER TABLE OfficialWork DROP COLUMN TechOfficialRating;

## v25.03
DROP TABLE ChiefJudgeReport;

CREATE TABLE ChiefJudgeReport (
SanctionId nchar(6) NOT NULL  
, RuleExceptions nvarchar(1024) NULL  
, RuleInterpretations nvarchar(1024) NULL  
, SafetyDirPerfReport nvarchar(1024) NULL  
, RopeHandlesSpecs nvarchar(1024) NULL  
, SlalomRopesSpecs nvarchar(1024) NULL  
, JumpRopesSpecs nvarchar(1024) NULL  
, SlalomCourseSpecs nvarchar(1024) NULL  
, JumpCourseSpecs nvarchar(1024) NULL  
, TrickCourseSpecs nvarchar(1024) NULL  
, BuoySpecs nvarchar(1024) NULL  
, RuleExceptQ1 nvarchar(128) NULL  
, RuleExceptQ2 nvarchar(128) NULL  
, RuleExceptQ3 nchar(1) NULL  
, RuleExceptQ4 nchar(1) NULL  
, RuleInterQ1 nvarchar(128) NULL  
, RuleInterQ2 nvarchar(128) NULL  
, RuleInterQ3 nchar(1) NULL  
, RuleInterQ4 nchar(1) NULL  
, LastUpdateDate datetime
);

ALTER TABLE ChiefJudgeReport ADD PRIMARY KEY (SanctionId);

Insert INTO ChiefJudgeReport (
SanctionId, RuleExceptions, RuleInterpretations, SafetyDirPerfReport
, RopeHandlesSpecs, SlalomRopesSpecs, JumpRopesSpecs, SlalomCourseSpecs, JumpCourseSpecs, TrickCourseSpecs, BuoySpecs
, RuleExceptQ1, RuleExceptQ2, RuleExceptQ3, RuleExceptQ4, RuleInterQ1, RuleInterQ2, RuleInterQ3, RuleInterQ4
, LastUpdateDate
) Select SanctionId, RuleExceptions, RuleInterpretations, SafetyDirPerfReport
, RopeHandlesSpecs, SlalomRopesSpecs, JumpRopesSpecs, SlalomCourseSpecs, JumpCourseSpecs, TrickCourseSpecs, BuoySpecs
, RuleExceptQ1, RuleExceptQ2, RuleExceptQ3, RuleExceptQ4, RuleInterQ1, RuleInterQ2, RuleInterQ3, RuleInterQ4
, LastUpdateDate 
From Tournament;


ALTER TABLE Tournament DROP COLUMN RuleExceptions;
ALTER TABLE Tournament DROP COLUMN RuleInterpretations;
ALTER TABLE Tournament DROP COLUMN SafetyDirPerfReport;
ALTER TABLE Tournament DROP COLUMN RopeHandlesSpecs;
ALTER TABLE Tournament DROP COLUMN SlalomRopesSpecs;
ALTER TABLE Tournament DROP COLUMN JumpRopesSpecs;
ALTER TABLE Tournament DROP COLUMN SlalomCourseSpecs;
ALTER TABLE Tournament DROP COLUMN JumpCourseSpecs;
ALTER TABLE Tournament DROP COLUMN TrickCourseSpecs;
ALTER TABLE Tournament DROP COLUMN BuoySpecs;
ALTER TABLE Tournament DROP COLUMN RuleExceptQ1;
ALTER TABLE Tournament DROP COLUMN RuleExceptQ2;
ALTER TABLE Tournament DROP COLUMN RuleExceptQ3;
ALTER TABLE Tournament DROP COLUMN RuleExceptQ4;
ALTER TABLE Tournament DROP COLUMN RuleInterQ1;
ALTER TABLE Tournament DROP COLUMN RuleInterQ2;
ALTER TABLE Tournament DROP COLUMN RuleInterQ3;
ALTER TABLE Tournament DROP COLUMN RuleInterQ4;

## v25.08
Update OfficialWorkAsgmt Set EventGroup = 'CM' Where EventGroup = 'Men A';
Update OfficialWorkAsgmt Set EventGroup = 'CW' Where EventGroup = 'Women A';
Update OfficialWorkAsgmt Set EventGroup = 'BM' Where EventGroup = 'Men B';
Update OfficialWorkAsgmt Set EventGroup = 'BW' Where EventGroup = 'Women B';