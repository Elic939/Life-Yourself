namespace PersonalLife.Core;
public sealed class FitnessService(IDataStore store)
{
 public WriteReceipt Save(Workout x){Validation.Workout(x);Validation.Require(!x.IsRestDay||!store.ReadAll().Exercises.Any(e=>e.WorkoutId==x.Id),"已有动作，不能直接改为休息日；请先处理训练记录");return store.Upsert(x);}
 public WriteReceipt Save(Exercise x){Validation.Exercise(x);var all=store.ReadAll();Validation.Require(all.Workouts.Any(w=>w.Id==x.WorkoutId&&!w.IsRestDay),"请先创建非休息日训练");Validation.Require(x.Kind=="strength"||!all.ExerciseSets.Any(s=>s.ExerciseId==x.Id),"已有力量组记录，不能改为按时长记录");return store.Upsert(x);}
 public WriteReceipt Save(ExerciseSet x){Validation.Set(x);Validation.Require(store.ReadAll().Exercises.Any(e=>e.Id==x.ExerciseId&&e.Kind=="strength"),"请先添加力量动作");return store.Upsert(x);}
 public List<Workout> GetForDate(DateOnly d)=>store.ReadAll().Workouts.Where(x=>x.Date==d).ToList();
}
