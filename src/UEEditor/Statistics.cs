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
@"�������:
	����������� ��������� - {0}
���������:
	�� ������������ - {1}
	�� ������������� - {2}
��������� ������� ���������: {3}
��������� ������������� ���������: {4}
��������� ������� ��������� ��������������: {5}
��������� ������������� ��������� ��������������: {6}

������������ �����?", 
				ForbiddenCount, SynonymCount, SynonymFirmCrCount, 
				HideSynonymCount, DuplicateSynonymCount, HideSynonymFirmCrCount, 
				DuplicateProducerSynonymCount);
		}
	}
}