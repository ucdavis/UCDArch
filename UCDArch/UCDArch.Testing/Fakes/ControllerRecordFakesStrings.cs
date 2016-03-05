using System.Collections.Generic;
using System.Linq;
using Rhino.Mocks;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;

namespace UCDArch.Testing.Fakes
{
    public abstract class ControllerRecordFakesStrings<T> where T : DomainObjectWithTypedId<string>
    {
        public void Records(int count, IRepositoryWithTypedId<T, string> repository, bool bypassSetIdTo)
        {
            var records = new List<T>();
            Records(count, repository, records, bypassSetIdTo);
        }


        public void Records(int count, IRepositoryWithTypedId<T, string> repository, List<T> specificRecords, bool bypassSetIdTo)
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

            var totalCount = records.Count;
            for (int i = 0; i < totalCount; i++)
            {
                if (!bypassSetIdTo)
                {
                    var stringId = (i + 1).ToString();
                    records[i].SetIdTo(stringId);
                    repository.Expect(a => a.GetNullableById(stringId))
                        .Return(records[i])
                        .Repeat
                        .Any();
                    repository.Expect(a => a.GetById(stringId))
                        .Return(records[i])
                        .Repeat
                        .Any();
                }
                else
                {
                    var i1 = i;
                    repository.Expect(a => a.GetNullableById(records[i1].Id))
                        .Return(records[i])
                        .Repeat
                        .Any();
                    repository.Expect(a => a.GetById(records[i1].Id))
                        .Return(records[i])
                        .Repeat
                        .Any();
                }
            }
            repository.Expect(a => a.GetNullableById((totalCount + 1).ToString())).Return(null).Repeat.Any();
            repository.Expect(a => a.GetById((totalCount + 1).ToString())).Return(null).Repeat.Any();
            repository.Expect(a => a.Queryable).Return(records.AsQueryable()).Repeat.Any();
            repository.Expect(a => a.GetAll()).Return(records).Repeat.Any();
        }

        protected abstract T CreateValid(int i);
    }
}