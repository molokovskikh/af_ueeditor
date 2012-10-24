using System;

namespace UEEditor
{
	public class Statistics
	{
		public int HideSynonymCount;
		public int HideSynonymFirmCrCount;
		public int DuplicateSynonymCount;
		public int DuplicateProducerSynonymCount;
		public int SynonymFirmCrCount;
		public int SynonymCount;
		public int ForbiddenCount;

		public void Reset()
		{
			SynonymCount = 0;
			SynonymFirmCrCount = 0;
			ForbiddenCount = 0;
			HideSynonymCount = 0;
			HideSynonymFirmCrCount = 0;
			DuplicateSynonymCount = 0;
			DuplicateProducerSynonymCount = 0;
		}

		public string Print()
		{
			return String.Format(
				@"Создано:
	запрещённых выражений - {0}
Синонимов:
	по наименованию - {1}
	по производителю - {2}
Отклонено скрытых синонимов: {3}
Отклонено дублирующихся синонимов: {4}
Отклонено скрытых синонимов производителей: {5}
Отклонено дублирующихся синонимов производителей: {6}

Перепровести прайс?",
				ForbiddenCount, SynonymCount, SynonymFirmCrCount,
				HideSynonymCount, DuplicateSynonymCount, HideSynonymFirmCrCount,
				DuplicateProducerSynonymCount);
		}
	}
}