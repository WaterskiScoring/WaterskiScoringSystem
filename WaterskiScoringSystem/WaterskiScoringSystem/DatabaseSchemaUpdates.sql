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

