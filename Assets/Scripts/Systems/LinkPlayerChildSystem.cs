using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


//public class LinkPlayerChildSystem : SystemBase // TODO: Hay que cambiar esto a un authoringcomponent.
//{
//    //[BurstDiscard]
//    //protected override void OnStartRunning()
//    //{
//    //    base.OnStartRunning();
        
//    //    Entities.WithAll<PlayerTag>().ForEach((in Entity entity, in DynamicBuffer<Child> childs) => {
//    //        DynamicBuffer<LinkedEntityGroup> linkedEntities = EntityManager.AddBuffer<LinkedEntityGroup>(entity);
//    //        linkedEntities.Add(new LinkedEntityGroup { Value = entity });
//    //        linkedEntities.Add(new LinkedEntityGroup { Value = childs.ElementAt(0).Value });
//    //        //child.ElementAt(0);
//    //        //child.Value;

//    //    }).WithoutBurst().Run();
//    }

//    protected override void OnUpdate()
//    {
        
//    }
//}
