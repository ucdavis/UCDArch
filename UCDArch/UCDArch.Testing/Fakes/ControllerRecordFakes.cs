using System.Collections.Generic;
using System.Linq;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;

namespace UCDArch.Testing.Fakes
{
    public abstract class ControllerRecordFakes<T> where T : DomainObject
    {
        public void Records(int count, IRepository<T> repository)
        {
            var records = new List<T>();
            Records(count, repository, records);
        }


        public void Records(int count, IRepository<T> repository, List<T> specificRecords)
        {
            var records = new List<T>();
            var specificRecordsCount = 0;
            if (specificRecords != null)
            {
                specificRecordsCount = specificRecords.Count;
                for (int i = 0; i < specificRecordsCount; i++)
                {
                    records.Add(specificRecords[i]);
                }
            }

            for (int i = 0; i < count; i++)
            {
                records.Add(CreateValid(i + specificRecordsCount + 1));
            }

            throw new System.NotImplementedException();
            // var totalCount = records.Count;
            // for (int i = 0; i < totalCount; i++)
            // {
            //     records[i].SetIdTo(i + 1);
            //     int i1 = i;
            //     repository
            //         .Expect(a => a.GetNullableById(i1 + 1))
            //         .Return(records[i])
            //         .Repeat
            //         .Any();
            // }
            // repository.Expect(a => a.GetNullableById(totalCount + 1)).Return(null).Repeat.Any();
            // repository.Expect(a => a.Queryable).Return(records.AsQueryable()).Repeat.Any();
            // repository.Expect(a => a.GetAll()).Return(records).Repeat.Any();
        }

        protected abstract T CreateValid(int i);
    }
}
