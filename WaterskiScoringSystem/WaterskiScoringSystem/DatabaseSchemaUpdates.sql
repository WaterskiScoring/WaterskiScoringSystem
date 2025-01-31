//------------------------------------------------------------
//Fix to ensure table has a valid value 
## v1.43
// Add status field
DROP TABLE TrickScoreBackup;
CREATE TABLE TrickScoreBackup (
    PK         bigint NOT NULL,
    SanctionId nchar(6) NOT NULL,
    MemberId   nchar(9) NOT NULL,
    AgeGroup   nvarchar(12) NOT NULL,
    Round      tinyint NOT NULL,
    Score      smallint,
    ScorePass1 smallint,
    ScorePass2 smallint,
    NopsScore  numeric(7,2),
    Rating     nvarchar(16),
    Boat       nvarchar(32),
    LastUpdateDate    datetime,
    Note       nvarchar(1024)
);

Insert into TrickScoreBackup (
    PK, SanctionId, MemberId, AgeGroup, Round, Score, ScorePass1, ScorePass2, NopsScore, Rating, Boat, LastUpdateDate, Note
) 
Select PK, SanctionId, MemberId, AgeGroup,  Round, Score, ScorePass1, ScorePass2, NopsScore, Rating, COALESCE(Boat, 'Undefined'), LastUpdateDate, Note 
From TrickScore S ;

Delete TrickScore ;
DROP TABLE TrickScore ;
CREATE TABLE TrickScore (
    PK         bigint NOT NULL IDENTITY,
    SanctionId nchar(6) NOT NULL,
    MemberId   nchar(9) NOT NULL,
    AgeGroup   nvarchar(12) NOT NULL,
    Round      tinyint NOT NULL,
    Score      smallint,
    ScorePass1 smallint,
    ScorePass2 smallint,
    NopsScore  numeric(7,2),
    Rating     nvarchar(16),
    Status     nvarchar(16),
    Boat       nvarchar(32),
    LastUpdateDate    datetime,
    Note       nvarchar(1024)
);
ALTER TABLE [TrickScore] ADD PRIMARY KEY ([PK]);
Insert into TrickScore (
    SanctionId, MemberId, AgeGroup, Round, Score, ScorePass1, ScorePass2, NopsScore, Rating, Status, Boat, LastUpdateDate, Note
) 
Select SanctionId, MemberId, AgeGroup,  Round, Score, ScorePass1, ScorePass2, NopsScore, Rating, 'Complete', Boat, LastUpdateDate, Note 
From TrickScoreBackup S ;

Delete TrickScoreBackup ;
DROP TABLE TrickScoreBackup ;

// Add status field
Delete From SlalomScore WHERE PK NOT IN (SELECT MIN(pk) AS Expr1 FROM SlalomScore as W WHERE SlalomScore.SanctionId = W.SanctionId AND SlalomScore.MemberId = W.MemberId AND SlalomScore.Round = W.Round);

//Delete From SlalomScoreBackup ;
DROP TABLE SlalomScoreBackup ;
CREATE TABLE SlalomScoreBackup (
    PK         bigint NOT NULL,
    SanctionId nchar(6) NOT NULL,
    MemberId   nchar(9) NOT NULL,
    AgeGroup   nvarchar(12) NOT NULL,
    Round      tinyint NOT NULL,
    MaxSpeed   tinyint,
    StartSpeed tinyint,
    StartLen   nvarchar(6),
    Score      numeric(5,2) ,
    NopsScore  numeric(7,2),
    Rating     nvarchar(16),
    Boat       nvarchar(32),
    LastUpdateDate    datetime,
    Note       nvarchar(1024)
);

Insert into SlalomScoreBackup (
    PK, SanctionId, MemberId, AgeGroup, Round, MaxSpeed, StartSpeed, StartLen, Score, NopsScore, Rating, Boat, LastUpdateDate, Note
) 
Select PK, SanctionId, MemberId, AgeGroup,  Round, MaxSpeed, StartSpeed, StartLen, Score, NopsScore, Rating, COALESCE(Boat, 'Undefined'), LastUpdateDate, Note 
From SlalomScore S ;

Delete From SlalomScore ;
DROP TABLE SlalomScore ;
CREATE TABLE SlalomScore (
    PK         bigint NOT NULL IDENTITY,
    SanctionId nchar(6) NOT NULL,
    MemberId   nchar(9) NOT NULL,
    AgeGroup   nvarchar(12) NOT NULL,
    Round      tinyint NOT NULL,
    MaxSpeed   tinyint,
    StartSpeed tinyint,
    StartLen   nvarchar(6),
    Score      numeric(5,2) ,
    NopsScore  numeric(7,2),
    Rating     nvarchar(16),
    Status     nvarchar(16),
    FinalPassNum tinyint,
    FinalSpeedMph tinyint,
    FinalSpeedKph tinyint,
    FinalLen   nvarchar(16),
    FinalLenOff nvarchar(16),
    FinalPassScore numeric(5,2) ,
    Boat       nvarchar(32),
    LastUpdateDate    datetime,
    Note       nvarchar(1024)
);
ALTER TABLE [SlalomScore] ADD PRIMARY KEY ([PK]);

Insert into SlalomScore (
    SanctionId, MemberId, AgeGroup, Round, MaxSpeed, StartSpeed, StartLen, Score, NopsScore, Rating, Status, Boat, LastUpdateDate, Note
) 
Select SanctionId, MemberId, AgeGroup,  Round, MaxSpeed, StartSpeed, StartLen, Score, NopsScore, Rating, 'Complete', Boat, LastUpdateDate, Note 
From SlalomScoreBackup ;

Delete From SlalomScoreBackup ;
DROP TABLE SlalomScoreBackup ;


// Add status field
Delete from JumpScore Where BoatSpeed is null OR RampHeight is null OR ScoreFeet is null OR ScoreMeters is null;
DROP TABLE JumpScoreBackup;
CREATE TABLE JumpScoreBackup (
    PK         bigint NOT NULL,
    SanctionId nchar(6) NOT NULL,
    MemberId   nchar(9) NOT NULL,
    AgeGroup   nvarchar(12) NOT NULL,
    Round      tinyint NOT NULL,
    BoatSpeed  tinyint,
    RampHeight numeric(3,1),
    ScoreFeet  numeric(4,1),
    ScoreMeters numeric(5,2),
    NopsScore  numeric(7,2),
    Rating     nvarchar(16),
    Boat       nvarchar(32),
    LastUpdateDate    datetime,
    Note       nvarchar(1024)
);
Insert into JumpScoreBackup (
    PK, SanctionId, MemberId, AgeGroup, Round, BoatSpeed, RampHeight, ScoreFeet, ScoreMeters, NopsScore, Rating, Boat, LastUpdateDate, Note
) 
Select PK, SanctionId, MemberId, AgeGroup,  Round, BoatSpeed, RampHeight, ScoreFeet, ScoreMeters, NopsScore, Rating, COALESCE(Boat, 'Undefined'), LastUpdateDate, Note 
From JumpScore S ;

Delete JumpScore ;
DROP TABLE JumpScore ;
CREATE TABLE JumpScore (
    PK         bigint NOT NULL IDENTITY,
    SanctionId nchar(6) NOT NULL,
    MemberId   nchar(9) NOT NULL,
    AgeGroup   nvarchar(12) NOT NULL,
    Round      tinyint NOT NULL,
    BoatSpeed  tinyint,
    RampHeight numeric(3,1),
    ScoreFeet  numeric(4,1),
    ScoreMeters numeric(5,2),
    NopsScore  numeric(7,2),
    Rating     nvarchar(16),
    Status     nvarchar(16),
    Boat       nvarchar(32),
    LastUpdateDate    datetime,
    Note       nvarchar(1024)
);
ALTER TABLE [JumpScore] ADD PRIMARY KEY ([PK]);
Insert into JumpScore (
    SanctionId, MemberId, AgeGroup, Round, BoatSpeed, RampHeight, ScoreFeet, ScoreMeters, NopsScore, Rating, Status, Boat, LastUpdateDate, Note
) 
Select SanctionId, MemberId, AgeGroup, Round, BoatSpeed, RampHeight, ScoreFeet, ScoreMeters, NopsScore, Rating, 'Complete', Boat, LastUpdateDate, Note  
From JumpScoreBackup S ;

Delete JumpScoreBackup ;
DROP TABLE JumpScoreBackup ;

//------------------------------------------------------------
## v1.58

ALTER TABLE [EventReg] ALTER COLUMN  RunOrder smallint ;

ALTER TABLE [SlalomRecap] ADD COLUMN  PassLineLength numeric(5,2);

//------------------------------------------------------------
## v1.62

ALTER TABLE [OfficialWorkAsgmt] ADD COLUMN  Round tinyint;

Update OfficialWorkAsgmt Set Round = 1 Where Round is null;

ALTER TABLE [OfficialWorkAsgmt] ALTER COLUMN Round tinyint NOT NULL;

Delete OfficialWorkAsgmt Where NOT EXISTS (Select 1 FROM CodeValueList AS L WHERE ListName = 'OfficialAsgmt' AND CodeValue = OfficialWorkAsgmt.WorkAsgmt);

//------------------------------------------------------------
## v1.64

DROP TABLE DivOrder ;

CREATE TABLE DivOrder (
    PK          bigint NOT NULL IDENTITY,
    SanctionId  nchar(6) NOT NULL,
    Event       nvarchar(12) NOT NULL,
    AgeGroup    nvarchar(12) NOT NULL,
    RunOrder    smallint,
    LastUpdateDate    datetime
);
ALTER TABLE [DivOrder] ADD PRIMARY KEY ([PK]);

//------------------------------------------------------------
## v1.74
Update Tournament set LastUpdateDate=GETDATE() Where LastUpdateDate is null;

DROP TABLE TourRegBackup ;
CREATE TABLE TourRegBackup (
    PK                bigint NOT NULL,
    MemberId          nchar(9) NOT NULL,
    SanctionId        nchar(6) NOT NULL,
    AgeGroup          nvarchar(12),
    SkierName         nvarchar(128),
    EntryDue          money,
    EntryPaid         money,
    PaymentMethod     nvarchar(64),
    ReadyToSki        nchar(1),
    AwsaMbrshpPaymt   money,
    AwsaMbrshpComment nvarchar(256),
    Weight            smallint,
    TrickBoat         nvarchar(64),
    JumpHeight        numeric(3,1),
    Federation        nvarchar(12),
    Gender            nchar(1),
    SkiYearAge        tinyint,
    State             nchar(2),
    LastUpdateDate    datetime,
    Notes             nvarchar(1024)
);

Insert into TourRegBackup (
    PK, SanctionId, MemberId, AgeGroup, SkierName
    , EntryDue, EntryPaid, PaymentMethod, ReadyToSki, AwsaMbrshpPaymt, AwsaMbrshpComment, Weight, TrickBoat, JumpHeight
    , Federation, Gender, SkiYearAge, State
    , LastUpdateDate, Notes
) 
Select PK, SanctionId, MemberId, AgeGroup,  SkierName
    , EntryDue, EntryPaid, PaymentMethod, ReadyToSki, AwsaMbrshpPaymt, AwsaMbrshpComment, Weight, TrickBoat, JumpHeight
    , Federation, Gender, SkiYearAge, State
	, LastUpdateDate, Notes 
From TourReg ;

Update TourRegBackup set AgeGroup = 'OF' where AgeGroup is null;

Delete TourReg ;
DROP TABLE TourReg ;
CREATE TABLE TourReg (
    PK                bigint NOT NULL IDENTITY,
    MemberId          nchar(9) NOT NULL,
    SanctionId        nchar(6) NOT NULL,
    AgeGroup          nvarchar(12) NOT NULL,
    SkierName         nvarchar(128),
    EntryDue          money,
    EntryPaid         money,
    PaymentMethod     nvarchar(64),
    ReadyToSki        nchar(1),
    AwsaMbrshpPaymt   money,
    AwsaMbrshpComment nvarchar(256),
    Weight            smallint,
    TrickBoat         nvarchar(64),
    JumpHeight        numeric(3,1),
    Federation        nvarchar(12),
    Gender            nchar(1),
    SkiYearAge        tinyint,
    State             nchar(2),
    LastUpdateDate    datetime,
    Notes             nvarchar(1024)
);
ALTER TABLE [TourReg] ADD PRIMARY KEY ([PK]) ;
Insert into TourReg (
	SanctionId, MemberId, AgeGroup, SkierName
    , EntryDue, EntryPaid, PaymentMethod, ReadyToSki, AwsaMbrshpPaymt, AwsaMbrshpComment, Weight, TrickBoat, JumpHeight
    , Federation, Gender, SkiYearAge, State
    , LastUpdateDate, Notes
) 
Select SanctionId, MemberId, COALESCE(AgeGroup, 'OF'),  SkierName
    , EntryDue, EntryPaid, PaymentMethod, ReadyToSki, AwsaMbrshpPaymt, AwsaMbrshpComment, Weight, TrickBoat, JumpHeight
    , Federation, Gender, SkiYearAge, State
	, LastUpdateDate, Notes 
From TourRegBackup ;

DROP TABLE TourRegBackup ;


//------------------------------------------------------------
//Updates to support additional scorebook placement methods
//Updates to add field on jump passes to know if speed was fast, slow, or good
## v1.75

ALTER TABLE [Tournament] ADD COLUMN PlcmtMethod nvarchar(256);
ALTER TABLE [Tournament] ADD COLUMN OverallMethod nvarchar(256);
ALTER TABLE [Tournament] ADD COLUMN ChiefScorerMemberId nchar(9);
ALTER TABLE [Tournament] ADD COLUMN ChiefScorerAddress nvarchar(128);
ALTER TABLE [Tournament] ADD COLUMN ChiefScorerPhone nvarchar(128);
ALTER TABLE [Tournament] ADD COLUMN ChiefScorerEmail nvarchar(128);
Update Tournament Set ChiefScorerMemberId = ContactMemberId;


ALTER TABLE [JumpRecap] ADD COLUMN  TimeInTol1 smallint;
ALTER TABLE [JumpRecap] ADD COLUMN  TimeInTol2 smallint;
ALTER TABLE [JumpRecap] ADD COLUMN  TimeInTol3 smallint;
Update JumpRecap Set TimeInTol1 = 0, TimeInTol2 = 0, TimeInTol3 = 0  Where TimeInTol1 is null And TimeInTol2 is null And TimeInTol3 is null;

ALTER TABLE [TourReg] ADD COLUMN ReadyForPlcmt nchar(1);
Update TourReg Set ReadyForPlcmt = 'Y' Where ReadyForPlcmt is null;

ALTER TABLE [SlalomScore] ADD COLUMN EventClass nchar(1);
ALTER TABLE [TrickScore] ADD COLUMN EventClass nchar(1);
ALTER TABLE [JumpScore] ADD COLUMN EventClass nchar(1);

Update SlalomScore Set EventClass = 'C'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = SlalomScore.SanctionId AND R.MemberId = SlalomScore.MemberId AND R.AgeGroup = SlalomScore.AgeGroup 
And R.Event = 'Slalom' And R.EventClass = 'C');

Update SlalomScore Set EventClass = 'E'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = SlalomScore.SanctionId AND R.MemberId = SlalomScore.MemberId AND R.AgeGroup = SlalomScore.AgeGroup 
And R.Event = 'Slalom' And R.EventClass = 'E');

Update SlalomScore Set EventClass = 'L'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = SlalomScore.SanctionId AND R.MemberId = SlalomScore.MemberId AND R.AgeGroup = SlalomScore.AgeGroup 
And R.Event = 'Slalom' And R.EventClass = 'L');

Update SlalomScore Set EventClass = 'R'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = SlalomScore.SanctionId AND R.MemberId = SlalomScore.MemberId AND R.AgeGroup = SlalomScore.AgeGroup 
And R.Event = 'Slalom' And R.EventClass = 'R');

Update SlalomScore Set EventClass = 'N'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = SlalomScore.SanctionId AND R.MemberId = SlalomScore.MemberId AND R.AgeGroup = SlalomScore.AgeGroup 
And R.Event = 'Slalom' And (R.EventClass = 'F' OR R.EventClass = 'N' OR R.EventClass = 'G'));

Update TrickScore Set EventClass = 'C'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = TrickScore.SanctionId AND R.MemberId = TrickScore.MemberId AND R.AgeGroup = TrickScore.AgeGroup 
And R.Event = 'Trick' And R.EventClass = 'C');

Update TrickScore Set EventClass = 'E'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = TrickScore.SanctionId AND R.MemberId = TrickScore.MemberId AND R.AgeGroup = TrickScore.AgeGroup 
And R.Event = 'Trick' And R.EventClass = 'E');

Update TrickScore Set EventClass = 'L'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = TrickScore.SanctionId AND R.MemberId = TrickScore.MemberId AND R.AgeGroup = TrickScore.AgeGroup 
And R.Event = 'Trick' And R.EventClass = 'L');

Update TrickScore Set EventClass = 'R'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = TrickScore.SanctionId AND R.MemberId = TrickScore.MemberId AND R.AgeGroup = TrickScore.AgeGroup 
And R.Event = 'Trick' And R.EventClass = 'R');

Update TrickScore Set EventClass = 'N'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = TrickScore.SanctionId AND R.MemberId = TrickScore.MemberId AND R.AgeGroup = TrickScore.AgeGroup 
And R.Event = 'Trick' And (R.EventClass = 'F' OR R.EventClass = 'N' OR R.EventClass = 'G'));

Update JumpScore Set EventClass = 'C'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = JumpScore.SanctionId AND R.MemberId = JumpScore.MemberId AND R.AgeGroup = JumpScore.AgeGroup 
And R.Event = 'Jump' And R.EventClass = 'C');

Update JumpScore Set EventClass = 'E'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = JumpScore.SanctionId AND R.MemberId = JumpScore.MemberId AND R.AgeGroup = JumpScore.AgeGroup 
And R.Event = 'Jump' And R.EventClass = 'E');

Update JumpScore Set EventClass = 'L'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = JumpScore.SanctionId AND R.MemberId = JumpScore.MemberId AND R.AgeGroup = JumpScore.AgeGroup 
And R.Event = 'Jump' And R.EventClass = 'L');

Update JumpScore Set EventClass = 'R'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = JumpScore.SanctionId AND R.MemberId = JumpScore.MemberId AND R.AgeGroup = JumpScore.AgeGroup 
And R.Event = 'Jump' And R.EventClass = 'R');

Update JumpScore Set EventClass = 'N'
Where exists (Select R.EventClass FROM EventReg AS R 
Where R.SanctionId = JumpScore.SanctionId AND R.MemberId = JumpScore.MemberId AND R.AgeGroup = JumpScore.AgeGroup 
And R.Event = 'Jump' And (R.EventClass = 'F' OR R.EventClass = 'N' OR R.EventClass = 'G'));

//------------------------------------------------------------
## v1.80

Delete TrickPass Where Code is null ;

ALTER TABLE [OfficialWork] ADD COLUMN TechChief nchar(1);
ALTER TABLE [OfficialWork] ADD COLUMN TechAsstChief nchar(1);
ALTER TABLE [OfficialWork] ADD COLUMN AnncrAsstChief nchar(1);

ALTER TABLE [JumpRecap] ADD COLUMN BoatSplitTimeTol tinyint;
ALTER TABLE [JumpRecap] ADD COLUMN BoatSplitTime2Tol tinyint;
ALTER TABLE [JumpRecap] ADD COLUMN BoatEndTimeTol tinyint;

//------------------------------------------------------------
//Build new tables for handling team order 
## v1.85
Drop TABLE TeamOrder;
CREATE TABLE TeamOrder (
    PK         bigint NOT NULL IDENTITY,
    SanctionId nchar(6) NOT NULL,
    TeamCode    nvarchar(12) NOT NULL,
    AgeGroup    nvarchar(12) NOT NULL,
    SlalomRunOrder smallint,
    TrickRunOrder  smallint,
    JumpRunOrder   smallint,
    SeedingScore  numeric(7,1),
    LastUpdateDate    datetime,
    Notes       nvarchar(1024)
);
ALTER TABLE [TeamOrder] ADD PRIMARY KEY ([PK]);

//------------------------------------------------------------
//build new tables for handling teams

DROP TABLE TeamListBackup;
CREATE TABLE TeamListBackup (
    PK         bigint NOT NULL,
    SanctionId nchar(6) NOT NULL,
    TeamCode    nvarchar(12) NOT NULL,
    Name        nvarchar(64),
    RunOrder    smallint,
    SlalomRunOrder smallint,
    TrickRunOrder  smallint,
    JumpRunOrder   smallint,
    ContactName nvarchar(128),
    ContactInfo nvarchar(128),
    LastUpdateDate    datetime,
    Notes       nvarchar(1024)
);

Insert into TeamListBackup (
PK, SanctionId, TeamCode, Name, RunOrder, SlalomRunOrder, TrickRunOrder, JumpRunOrder, ContactName, ContactInfo, LastUpdateDate, Notes
) 
Select  PK, SanctionId, TeamCode, Name, RunOrder, SlalomRunOrder, TrickRunOrder, JumpRunOrder, ContactName, ContactInfo, LastUpdateDate, Notes 
From TeamList;
Delete From TeamList ;

Delete from TeamListBackup Where TeamCode is null;
Delete from TeamListBackup Where TeamCode = '';
Delete from TeamListBackup Where TeamCode = 'OFF';

Update TeamListBackup
	Set SlalomRunOrder = RunOrder 
Where SlalomRunOrder is null OR SlalomRunOrder = 0;
Update TeamListBackup
	Set TrickRunOrder = RunOrder 
Where TrickRunOrder is null OR TrickRunOrder = 0;
Update TeamListBackup
	Set JumpRunOrder = RunOrder 
Where JumpRunOrder is null OR JumpRunOrder = 0;

Drop TABLE TeamList;
CREATE TABLE TeamList (
    PK         bigint NOT NULL IDENTITY,
    SanctionId nchar(6) NOT NULL,
    TeamCode    nvarchar(12) NOT NULL,
    Name        nvarchar(64),
    ContactName nvarchar(128),
    ContactInfo nvarchar(128),
    LastUpdateDate    datetime,
    Notes       nvarchar(1024)
);
ALTER TABLE [TeamList] ADD PRIMARY KEY ([PK]);

Insert into TeamList (
SanctionId, TeamCode, Name, ContactName, ContactInfo, LastUpdateDate, Notes
)  
Select SanctionId, TeamCode, Name, ContactName, ContactInfo, LastUpdateDate, Notes From TeamListBackup;

Insert into TeamOrder (
	SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate 
) 
Select SanctionId, TeamCode, 'CM', SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate 
From TeamListBackup L 
Where EXISTS (Select 1 From Tournament AS T Where T.SanctionId = L.SanctionId AND Rules = 'ncwsa');

Insert into TeamOrder (
	SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate
) 
Select SanctionId, TeamCode, 'CW', SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate 
From TeamListBackup L 
Where EXISTS (Select 1 From Tournament AS T Where T.SanctionId = L.SanctionId AND Rules = 'ncwsa');

Insert into TeamOrder (
	SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate
) 
Select SanctionId, TeamCode, 'BM', SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate 
From TeamListBackup L 
Where EXISTS (Select 1 From Tournament AS T Where T.SanctionId = L.SanctionId AND Rules = 'ncwsa');

Insert into TeamOrder (
	SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate
) 
Select SanctionId, TeamCode, 'BW', SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate 
From TeamListBackup L 
Where EXISTS (Select 1 From Tournament AS T Where T.SanctionId = L.SanctionId AND Rules = 'ncwsa');

Insert into TeamOrder (
	SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate 
) 
Select SanctionId, TeamCode, '', SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate 
From TeamListBackup L 
Where EXISTS (Select 1 From Tournament AS T Where T.SanctionId = L.SanctionId AND Rules <> 'ncwsa');

DROP TABLE TeamListBackup;

//------------------------------------------------------------
//Build new tables for handling team order 
## v1.90
Drop TABLE EventRunOrder;
CREATE TABLE EventRunOrder (
    PK         bigint NOT NULL IDENTITY,
    SanctionId nchar(6) NOT NULL,
    MemberId   nchar(9) NOT NULL,
    AgeGroup   nvarchar(12) NOT NULL,
    EventGroup nvarchar(12) NOT NULL,
    Event      nvarchar(12) NOT NULL,
    Round      tinyint NOT NULL,
    RunOrder   smallint,
    LastUpdateDate datetime,
    Notes      nvarchar(1024)
);
ALTER TABLE [EventRunOrder] ADD PRIMARY KEY ([PK]);


//------------------------------------------------------------
## v1.92

DROP TABLE TourRegBackup ;
CREATE TABLE TourRegBackup (
    PK                bigint NOT NULL,
    MemberId          nchar(9) NOT NULL,
    SanctionId        nchar(6) NOT NULL,
    AgeGroup          nvarchar(12) NOT NULL,
    SkierName         nvarchar(128),
    EntryDue          money,
    EntryPaid         money,
    PaymentMethod     nvarchar(64),
    ReadyToSki        nchar(1),
    ReadyForPlcmt     nchar(1),
    AwsaMbrshpPaymt   money,
    AwsaMbrshpComment nvarchar(256),
    Weight            smallint,
    TrickBoat         nvarchar(64),
    JumpHeight        numeric(3,1),
    Federation        nvarchar(12),
    Gender            nchar(1),
    SkiYearAge        tinyint,
    State             nchar(2),
    LastUpdateDate    datetime,
    Notes             nvarchar(1024)

);

Insert into TourRegBackup (
    PK, SanctionId, MemberId, AgeGroup, SkierName
    , EntryDue, EntryPaid, PaymentMethod, ReadyToSki, ReadyForPlcmt, AwsaMbrshpPaymt, AwsaMbrshpComment, Weight, TrickBoat, JumpHeight
    , Federation, Gender, SkiYearAge, State, LastUpdateDate, Notes
) 
Select PK, SanctionId, MemberId, COALESCE(AgeGroup, 'OF'),  SkierName
    , EntryDue, EntryPaid, PaymentMethod, ReadyToSki, ReadyForPlcmt, AwsaMbrshpPaymt, AwsaMbrshpComment, Weight, TrickBoat, JumpHeight
    , Federation, Gender, SkiYearAge, State, LastUpdateDate, Notes 
From TourReg ;

Delete TourReg ;
DROP TABLE TourReg ;
CREATE TABLE TourReg (
    PK                bigint NOT NULL IDENTITY,
    MemberId          nchar(9) NOT NULL,
    SanctionId        nchar(6) NOT NULL,
    AgeGroup          nvarchar(12) NOT NULL,
    SkierName         nvarchar(128),
    EntryDue          money,
    EntryPaid         money,
    PaymentMethod     nvarchar(64),
    ReadyToSki        nchar(1),
    ReadyForPlcmt     nchar(1),
    AwsaMbrshpPaymt   money,
    AwsaMbrshpComment nvarchar(256),
    Weight            smallint,
    TrickBoat         nvarchar(64),
    JumpHeight        numeric(3,1),
    Federation        nvarchar(12),
    Gender            nchar(1),
    SkiYearAge        tinyint,
    State             nchar(2),
    LastUpdateDate    datetime,
    Notes             nvarchar(1024)
);
ALTER TABLE [TourReg] ADD PRIMARY KEY ([PK]) ;
Insert into TourReg (
	SanctionId, MemberId, AgeGroup, SkierName
    , EntryDue, EntryPaid, PaymentMethod, ReadyToSki, ReadyForPlcmt, AwsaMbrshpPaymt, AwsaMbrshpComment, Weight, TrickBoat, JumpHeight
    , Federation, Gender, SkiYearAge, State
    , LastUpdateDate, Notes
) 
Select SanctionId, MemberId, AgeGroup,  SkierName
    , EntryDue, EntryPaid, PaymentMethod, ReadyToSki, ReadyForPlcmt, AwsaMbrshpPaymt, AwsaMbrshpComment, Weight, TrickBoat, JumpHeight
    , Federation, Gender, SkiYearAge, State
	, LastUpdateDate, Notes 
From TourRegBackup ;

DROP TABLE TourRegBackup ;
Update TourReg Set ReadyForPlcmt = 'Y' Where ReadyForPlcmt is null;


//------------------------------------------------------------
## v1.94

ALTER TABLE [JumpRecap] DROP COLUMN  TimeInTol1;
ALTER TABLE [JumpRecap] DROP COLUMN  TimeInTol2;
ALTER TABLE [JumpRecap] DROP COLUMN  TimeInTol3;
ALTER TABLE [JumpRecap] DROP COLUMN BoatSplitTimeTol;
ALTER TABLE [JumpRecap] DROP COLUMN BoatSplitTime2Tol;
ALTER TABLE [JumpRecap] DROP COLUMN BoatEndTimeTol;

ALTER TABLE [JumpRecap] ADD COLUMN  Split52TimeTol smallint;
ALTER TABLE [JumpRecap] ADD COLUMN  Split82TimeTol smallint;
ALTER TABLE [JumpRecap] ADD COLUMN  Split41TimeTol smallint;

//------------------------------------------------------------
## v2.09

ALTER TABLE [TourReg] ADD COLUMN City nvarchar(128);

//------------------------------------------------------------
## v2.10

UPDATE EventReg SET EventClass = 'F' 
WHERE NOT EXISTS (SELECT 1 AS Expr1 FROM CodeValueList WHERE ListCode = EventClass AND ListName = 'Class');

//------------------------------------------------------------
## v2.20
CREATE TABLE TourProperties (
    PK bigint NOT NULL IDENTITY,
    SanctionId nchar(6) NOT NULL,
    PropKey nvarchar(1024) NOT NULL,
    PropOrder smallint,
    PropValue nvarchar(1024)
);
ALTER TABLE [TourProperties] ADD PRIMARY KEY ([PK]);

//------------------------------------------------------------
## v2.23
ALTER TABLE [EventRunOrder] ADD COLUMN RankingScore  numeric(9,3);

//------------------------------------------------------------
## v2.24
ALTER TABLE [TrickList] ADD COLUMN Description nvarchar(128);

//------------------------------------------------------------
## v2.27
ALTER TABLE [MemberList] ADD COLUMN JudgeSlalomRating nvarchar(4);
ALTER TABLE [MemberList] ADD COLUMN JudgeTrickRating nvarchar(4);
ALTER TABLE [MemberList] ADD COLUMN JudgeJumpRating nvarchar(4);
ALTER TABLE [MemberList] ADD COLUMN DriverSlalomRating nvarchar(4);
ALTER TABLE [MemberList] ADD COLUMN DriverTrickRating nvarchar(4);
ALTER TABLE [MemberList] ADD COLUMN DriverJumpRating nvarchar(4);
ALTER TABLE [MemberList] ADD COLUMN ScorerSlalomRating nvarchar(4);
ALTER TABLE [MemberList] ADD COLUMN ScorerTrickRating nvarchar(4);
ALTER TABLE [MemberList] ADD COLUMN ScorerJumpRating nvarchar(4);

ALTER TABLE [OfficialWork] ADD COLUMN JudgeSlalomRating nvarchar(32);
ALTER TABLE [OfficialWork] ADD COLUMN JudgeTrickRating nvarchar(32);
ALTER TABLE [OfficialWork] ADD COLUMN JudgeJumpRating nvarchar(32);
ALTER TABLE [OfficialWork] ADD COLUMN DriverSlalomRating nvarchar(32);
ALTER TABLE [OfficialWork] ADD COLUMN DriverTrickRating nvarchar(32);
ALTER TABLE [OfficialWork] ADD COLUMN DriverJumpRating nvarchar(32);
ALTER TABLE [OfficialWork] ADD COLUMN ScorerSlalomRating nvarchar(32);
ALTER TABLE [OfficialWork] ADD COLUMN ScorerTrickRating nvarchar(32);
ALTER TABLE [OfficialWork] ADD COLUMN ScorerJumpRating nvarchar(32);

Update MemberList Set 
JudgeSlalomRating = SlalomOfficialRating
, JudgeTrickRating = TrickOfficialRating
, JudgeJumpRating = JumpOfficialRating
, DriverSlalomRating = DriverOfficialRating
, DriverTrickRating = DriverOfficialRating
, DriverJumpRating = DriverOfficialRating
, ScorerSlalomRating = ScoreOfficialRating
, ScorerTrickRating = ScoreOfficialRating
, ScorerJumpRating = ScoreOfficialRating
;

Update OfficialWork Set 
JudgeSlalomRating = SlalomOfficialRating
, JudgeTrickRating = TrickOfficialRating
, JudgeJumpRating = JumpOfficialRating
, DriverSlalomRating = DriverOfficialRating
, DriverTrickRating = DriverOfficialRating
, DriverJumpRating = DriverOfficialRating
, ScorerSlalomRating = ScoreOfficialRating
, ScorerTrickRating = ScoreOfficialRating
, ScorerJumpRating = ScoreOfficialRating
;

ALTER TABLE [MemberList] DROP COLUMN SlalomOfficialRating;
ALTER TABLE [MemberList] DROP COLUMN TrickOfficialRating;
ALTER TABLE [MemberList] DROP COLUMN JumpOfficialRating;
ALTER TABLE [MemberList] DROP COLUMN ScoreOfficialRating;
ALTER TABLE [MemberList] DROP COLUMN DriverOfficialRating;

ALTER TABLE [OfficialWork] DROP COLUMN SlalomOfficialRating;
ALTER TABLE [OfficialWork] DROP COLUMN TrickOfficialRating;
ALTER TABLE [OfficialWork] DROP COLUMN JumpOfficialRating;
ALTER TABLE [OfficialWork] DROP COLUMN ScoreOfficialRating;
ALTER TABLE [OfficialWork] DROP COLUMN DriverOfficialRating;

//------------------------------------------------------------
## v3.05
ALTER TABLE [TrickScore] ADD COLUMN Pass1VideoUrl nvarchar(256);
ALTER TABLE [TrickScore] ADD COLUMN Pass2VideoUrl nvarchar(256);

//------------------------------------------------------------
## v3.12
DROP TABLE TrickVideo ;
CREATE TABLE TrickVideo (
    PK         bigint NOT NULL IDENTITY,
    SanctionId nchar(6) NOT NULL,
    MemberId   nchar(9) NOT NULL,
    AgeGroup   nvarchar(12) NOT NULL,
    Round      tinyint NOT NULL,
	Pass1VideoUrl nvarchar(256),
	Pass2VideoUrl nvarchar(256),
    LastUpdateDate    datetime
);
ALTER TABLE [TrickVideo] ADD PRIMARY KEY ([PK]);

Insert into TrickVideo (
    SanctionId, MemberId, AgeGroup, Round, Pass1VideoUrl, Pass2VideoUrl, LastUpdateDate
) Select SanctionId, MemberId, AgeGroup,  Round, Pass1VideoUrl, Pass2VideoUrl, LastUpdateDate From TrickScore S Where LEN(Pass1VideoUrl) > 1 OR LEN(Pass2VideoUrl) > 1;

ALTER TABLE [TrickScore] DROP COLUMN Pass1VideoUrl;
ALTER TABLE [TrickScore] DROP COLUMN Pass2VideoUrl;

//------------------------------------------------------------
## v3.23
ALTER TABLE [TeamOrder] ADD COLUMN EventGroup nvarchar(12);
ALTER TABLE [EventReg] ADD COLUMN  Rotation smallint ;

//------------------------------------------------------------
## v3.32
Update EventReg set EventClass = 'F' Where EventClass = 'N';
Update SlalomScore set EventClass = 'F' Where EventClass = 'N';
Update TrickScore set EventClass = 'F' Where EventClass = 'N';
Update JumpScore set EventClass = 'F' Where EventClass = 'N';


//------------------------------------------------------------
## v3.51
ALTER TABLE [TrickList] ALTER COLUMN RuleNum smallint;

//------------------------------------------------------------
## v4.04
ALTER TABLE [SlalomScore] ADD COLUMN CompletedSpeedMph tinyint;
ALTER TABLE [SlalomScore] ADD COLUMN CompletedSpeedKph tinyint;

//------------------------------------------------------------
## v4.09
Update TourReg Set ReadyForPlcmt = 'Y' Where ReadyForPlcmt is null;

//------------------------------------------------------------
## v4.10
ALTER TABLE [EventReg] ADD COLUMN ReadyForPlcmt nchar(1);
Update EventReg Set ReadyForPlcmt = 'Y' Where ReadyForPlcmt is null;
Update EventReg Set ReadyForPlcmt = 'N' Where EventGroup = 'RL';

//------------------------------------------------------------
## v4.14
ALTER TABLE [MemberList] ALTER COLUMN JudgeSlalomRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN JudgeTrickRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN JudgeJumpRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN DriverSlalomRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN DriverTrickRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN DriverJumpRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN ScorerSlalomRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN ScorerTrickRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN ScorerJumpRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN SafetyOfficialRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN TechOfficialRating nvarchar(32);
ALTER TABLE [MemberList] ALTER COLUMN AnncrOfficialRating nvarchar(32);

ALTER TABLE [Tournament] ADD COLUMN SanctionEditCode nvarchar(32);

Update MemberList Set JudgeSlalomRating = 'Assistant'  Where JudgeSlalomRating = 'A';
Update MemberList Set JudgeSlalomRating = 'Regular'  Where JudgeSlalomRating = 'R';
Update MemberList Set JudgeSlalomRating = 'Senior'  Where JudgeSlalomRating = 'S';
Update MemberList Set JudgeSlalomRating = 'Emeritus '  Where JudgeSlalomRating = 'E';
Update MemberList Set JudgeSlalomRating = 'PanAm'  Where JudgeSlalomRating = 'P';

Update MemberList Set JudgeTrickRating = 'Assistant'  Where JudgeTrickRating = 'A';
Update MemberList Set JudgeTrickRating = 'Regular'  Where JudgeTrickRating = 'R';
Update MemberList Set JudgeTrickRating = 'Senior'  Where JudgeTrickRating = 'S';
Update MemberList Set JudgeTrickRating = 'Emeritus '  Where JudgeTrickRating = 'E';
Update MemberList Set JudgeTrickRating = 'PanAm'  Where JudgeTrickRating = 'P';

Update MemberList Set JudgeJumpRating = 'Assistant'  Where JudgeJumpRating = 'A';
Update MemberList Set JudgeJumpRating = 'Regular'  Where JudgeJumpRating = 'R';
Update MemberList Set JudgeJumpRating = 'Senior'  Where JudgeJumpRating = 'S';
Update MemberList Set JudgeJumpRating = 'Emeritus '  Where JudgeJumpRating = 'E';
Update MemberList Set JudgeJumpRating = 'PanAm'  Where JudgeJumpRating = 'P';

Update MemberList Set DriverTrickRating = 'Trained'  Where DriverTrickRating = 'T';	
Update MemberList Set DriverSlalomRating = 'Trained'  Where DriverSlalomRating = 'T';	
Update MemberList Set DriverJumpRating = 'Trained'  Where DriverJumpRating = 'T';
Update MemberList Set DriverTrickRating = 'Assistant'  Where DriverTrickRating = 'A';	
Update MemberList Set DriverSlalomRating = 'Assistant'  Where DriverSlalomRating = 'A';	
Update MemberList Set DriverJumpRating = 'Assistant'  Where DriverJumpRating = 'A';
Update MemberList Set DriverTrickRating = 'Regular'  Where DriverTrickRating = 'R';	
Update MemberList Set DriverSlalomRating = 'Regular'  Where DriverSlalomRating = 'R';	
Update MemberList Set DriverJumpRating = 'Regular'  Where DriverJumpRating = 'R';
Update MemberList Set DriverTrickRating = 'Senior'  Where DriverTrickRating = 'S';
Update MemberList Set DriverSlalomRating = 'Senior'  Where DriverSlalomRating = 'S';
Update MemberList Set DriverJumpRating = 'Senior'  Where DriverJumpRating = 'S';
Update MemberList Set DriverTrickRating = 'Emeritus'  Where DriverTrickRating = 'E';	
Update MemberList Set DriverSlalomRating = 'Emeritus'  Where DriverSlalomRating = 'E';	
Update MemberList Set DriverJumpRating = 'Emeritus'  Where DriverJumpRating = 'E';

Update MemberList Set ScorerTrickRating = 'Assistant'  Where ScorerTrickRating = 'A';	
Update MemberList Set ScorerSlalomRating = 'Assistant'  Where ScorerSlalomRating = 'A';	
Update MemberList Set ScorerJumpRating = 'Assistant'  Where ScorerJumpRating = 'A';
Update MemberList Set ScorerTrickRating = 'Regular'  Where ScorerTrickRating = 'R';	
Update MemberList Set ScorerSlalomRating = 'Regular'  Where ScorerSlalomRating = 'R';	
Update MemberList Set ScorerJumpRating = 'Regular'  Where ScorerJumpRating = 'R';
Update MemberList Set ScorerTrickRating = 'Senior'  Where ScorerTrickRating = 'S';	
Update MemberList Set ScorerSlalomRating = 'Senior'  Where ScorerSlalomRating = 'S';	
Update 
Set ScorerJumpRating = 'Senior'  Where ScorerJumpRating = 'S';
Update MemberList Set ScorerTrickRating = 'Emeritus'  Where ScorerTrickRating = 'E';	
Update MemberList Set ScorerSlalomRating = 'Emeritus'  Where ScorerSlalomRating = 'E';	
Update MemberList Set ScorerJumpRating = 'Emeritus'  Where ScorerJumpRating = 'E';

Update MemberList Set SafetyOfficialRating = 'Coordinator'  Where SafetyOfficialRating= 'C';
Update MemberList Set SafetyOfficialRating = 'State'  Where SafetyOfficialRating= 'S';
Update MemberList Set SafetyOfficialRating = 'Regional'  Where SafetyOfficialRating= 'R';
Update MemberList Set SafetyOfficialRating = 'National'  Where SafetyOfficialRating= 'N';
Update MemberList Set SafetyOfficialRating = 'Emeritus'  Where SafetyOfficialRating= 'E';

Update MemberList Set TechOfficialRating = 'Assistant'  Where TechOfficialRating = 'A';
Update MemberList Set TechOfficialRating = 'Regular'  Where TechOfficialRating = 'R';
Update MemberList Set TechOfficialRating = 'Senior'  Where TechOfficialRating = 'S';
Update 
Set TechOfficialRating = 'Emeritus'  Where TechOfficialRating = 'E';

Update MemberList Set AnncrOfficialRating = 'State'  Where AnncrOfficialRating = 'S';
Update MemberList Set AnncrOfficialRating = 'Regional'  Where AnncrOfficialRating = 'R';
Update MemberList Set AnncrOfficialRating = 'National'  Where AnncrOfficialRating = 'N';
Update MemberList Set AnncrOfficialRating = 'Emeritus'  Where AnncrOfficialRating = 'E';

Update OfficialWork Set JudgeSlalomRating = '' Where JudgeSlalomRating = 'Unrated';
Update OfficialWork Set JudgeTrickRating = '' Where JudgeTrickRating = 'Unrated';
Update OfficialWork Set JudgeJumpRating = '' Where JudgeJumpRating = 'Unrated';
Update OfficialWork Set DriverSlalomRating = '' Where DriverSlalomRating = 'Unrated';
Update OfficialWork Set DriverTrickRating = '' Where DriverTrickRating = 'Unrated';
Update OfficialWork Set DriverJumpRating = '' Where DriverJumpRating = 'Unrated';
Update OfficialWork Set ScorerSlalomRating = '' Where ScorerSlalomRating = 'Unrated';
Update OfficialWork Set ScorerTrickRating = '' Where ScorerTrickRating = 'Unrated';
Update OfficialWork Set ScorerJumpRating = '' Where ScorerJumpRating = 'Unrated';
Update OfficialWork Set SafetyOfficialRating = '' Where SafetyOfficialRating = 'Unrated';
Update OfficialWork Set TechOfficialRating = '' Where TechOfficialRating = 'Unrated';

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

DROP TABLE ChiefJudgeReport;

CREATE TABLE ChiefJudgeReport (
SanctionId nchar(6) NOT NULL  
, Rules nvarchar(16) NULL  
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
