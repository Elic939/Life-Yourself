namespace PersonalLife.Core;
public interface IEntity { Guid Id { get; } }
public sealed record DailyTask : IEntity { public Guid Id {get;init;}=Guid.NewGuid(); public DateOnly Date {get;init;} public string Title {get;init;}=""; public TimeOnly? Time {get;init;} public string Priority {get;init;}="normal"; public bool IsCompleted {get;init;} }
public sealed record Workout : IEntity { public Guid Id {get;init;}=Guid.NewGuid(); public DateOnly Date {get;init;} public string Title {get;init;}=""; public TimeOnly? StartTime {get;init;} public int? PlannedMinutes {get;init;} public bool IsRestDay {get;init;} public bool IsCompleted {get;init;} }
public sealed record Exercise : IEntity { public Guid Id {get;init;}=Guid.NewGuid(); public Guid WorkoutId {get;init;} public string Name {get;init;}=""; public string Kind {get;init;}="strength"; public int SortOrder {get;init;} public int? PlannedSets {get;init;} public int? PlannedReps {get;init;} public decimal? PlannedWeightKg {get;init;} public int? PlannedMinutes {get;init;} public int? ActualMinutes {get;init;} public bool IsCompleted {get;init;} }
public sealed record ExerciseSet : IEntity { public Guid Id {get;init;}=Guid.NewGuid(); public Guid ExerciseId {get;init;} public int SetNumber {get;init;} public int ActualReps {get;init;} public decimal? ActualWeightKg {get;init;} }
public sealed record Meal : IEntity { public Guid Id {get;init;}=Guid.NewGuid(); public DateOnly Date {get;init;} public string Slot {get;init;}="breakfast"; public string PlannedText {get;init;}=""; public string ActualText {get;init;}=""; public bool IsRecorded {get;init;} }
public sealed record WaterEntry : IEntity { public Guid Id {get;init;}=Guid.NewGuid(); public DateOnly Date {get;init;} public int Milliliters {get;init;} }
public sealed record GamePlan : IEntity { public Guid Id {get;init;}=Guid.NewGuid(); public DateOnly Date {get;init;} public string GameName {get;init;}=""; public TimeOnly StartTime {get;init;} public int PlannedMinutes {get;init;} }
public sealed record GameSession : IEntity { public Guid Id {get;init;}=Guid.NewGuid(); public DateOnly Date {get;init;} public string GameName {get;init;}=""; public string Mode {get;init;}="manual"; public DateTimeOffset? StartedAt {get;init;} public DateTimeOffset? EndedAt {get;init;} public int DurationMinutes {get;init;} }
public sealed record JournalEntry : IEntity { public Guid Id {get;init;}=Guid.NewGuid(); public DateOnly Date {get;init;} public string? Mood {get;init;} public string Summary {get;init;}=""; public string Gains {get;init;}=""; public string Improvements {get;init;}=""; public DateTimeOffset UpdatedAt {get;init;} }
public sealed record UserSettings { public string WeekStartsOn {get;init;}="monday"; }
public sealed record AppSnapshot
{
 public List<DailyTask> Tasks {get;init;}=[];
 public List<Workout> Workouts {get;init;}=[];
 public List<Exercise> Exercises {get;init;}=[];
 public List<ExerciseSet> ExerciseSets {get;init;}=[];
 public List<Meal> Meals {get;init;}=[];
 public List<WaterEntry> WaterEntries {get;init;}=[];
 public List<GamePlan> GamePlans {get;init;}=[];
 public List<GameSession> GameSessions {get;init;}=[];
 public List<JournalEntry> Journals {get;init;}=[];
 public UserSettings Settings {get;init;}=new();
 public IEnumerable<IEntity> Entities()=>Tasks.Cast<IEntity>().Concat(Workouts).Concat(Exercises).Concat(ExerciseSets).Concat(Meals).Concat(WaterEntries).Concat(GamePlans).Concat(GameSessions).Concat(Journals);
}
public sealed record WriteReceipt(long Revision);
