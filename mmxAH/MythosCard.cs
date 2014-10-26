using System;
using System.Collections.Generic;

namespace mmxAH
{
	public abstract  class MythosCard: Card
	{ protected GameEngine en;
		private short GateLoc, ClueLoc;
		private List<byte> MoveBlack, MoveWhite;
		private string Title;
		public MythosCard ( GameEngine eng, short pID)
		{
			en = eng;
			ID = pID;
			MoveBlack = new List<byte> ();
			MoveWhite = new List<byte> ();  
		}

		public void Execute()
		{ en.curs.resolvingMythos=this; 
		  en.clock.PrintCurPhase ();
			en.io.ServerWrite ( Environment.NewLine+ Title+ "  ", 16, true); 
			PrintType (); 

			en.io.ServerWrite (Environment.NewLine+ en.sysstr.GetString (SSType.MythosStep1)+Environment.NewLine, 12, true ); 
		
			ArchamUnstableLoc l = (ArchamUnstableLoc)en.locs [GateLoc];  

			if (l.isGate ())
				en.ga.MonsterSurge (GateLoc);
			else
			{
				if (! l.OpenGate ())
					return;
				Step2 ();
			}




		}

		private void Step2()
		{ 
			en.io.ServerWrite (Environment.NewLine+ en.sysstr.GetString (SSType.MythosStep2)+Environment.NewLine, 12, true ); 
			((ArchamArea)en.locs [ClueLoc]).MythosClues ();   
		}

		public  void Step3()
		{
			en.io.ServerWrite (Environment.NewLine+ en.sysstr.GetString (SSType.MythosStep3)+Environment.NewLine, 12, true ); 
			foreach (MonsterIndivid m  in en.ActiveMonsters)
				m.isEncountred = false;
			Step3Circle ();
		}

		public void Step3Circle()
		{ foreach (MonsterIndivid m in en.ActiveMonsters)
				if (! m.isEncountred)
			{  m.isEncountred = true; 
				byte dsi = m.GetDs ();

					if (MoveBlack.IndexOf (dsi) >= 0)
				{ m.Move (false); return;
					}
				   if (MoveWhite.IndexOf (dsi) >= 0)
				{ m.Move (true); return;
				   }
				  
				// не двигаемся
					Step3Circle ();
					return;

				}

			// нет больше монстров для движения
			Step4 ();

		}

		protected abstract void Step4();
		protected abstract  void PrintType();

		public virtual bool  FromTextFile(TextFileParser data, TextFileParser text)
		{  Title = text.GetCurString (); 
			GateLoc  = en.map.GetNumberByCodeName (data.GetToken());
			ClueLoc  = en.map.GetNumberByCodeName (data.GetToken());
			if (GateLoc == -1 || ClueLoc == -1)
				return false;
			if (en.locs [GateLoc].GetLocType () != LocathionType.ArchamUnstable)
				return false;
			short count, b;
			if (! short.TryParse (data.GetToken(), out count))
				return false;
			for (short i=0; i< count; i++)
			{
				b = (short) en.ds.GetIndex (data.GetToken ());
				if (b == -1)
					return false;
				MoveWhite.Add ((byte)b);
			}

			if (! short.TryParse (data.GetToken(), out count))
				return false;
			for (short i=0; i< count; i++)
			{
				b = (short) en.ds.GetIndex (data.GetToken ());
				if (b == -1)
					return false;
				MoveBlack.Add ((byte)b);
			}


			return true;

		}



	}



	public class MythosHead : MythosCard
	{  private  Effect eff;
		public MythosHead ( GameEngine eng, short pID) : base(eng, pID)
		{

			//заглушка
			eff = new EffNothing (en); 

		}


		protected override void Step4 ()
		{  en.io.ServerWrite (Environment.NewLine+ en.sysstr.GetString (SSType.MythosStep4)+Environment.NewLine, 12, true ); 
			eff.Execute( en.clock.EndTurn);  

		}

		protected override void PrintType ()
		{
			en.io.ServerWrite ("(Headline)" + Environment.NewLine, 14, true, true);
		}
	}


	public class MythosEnv : MythosCard
	{ public MythosEnv ( GameEngine eng, short pID) : base(eng, pID)
		{

		}


		protected override void Step4 ()
		{ en.io.ServerWrite (Environment.NewLine+ en.sysstr.GetString (SSType.MythosStep4)+Environment.NewLine, 12, true ); 
			if (en.curs.curEnv != null)
			{
				en.curs.curEnv.Stop ();
				en.mythosDeck.Discard (en.curs.curEnv);
			}
			en.curs.curEnv = this;
			Start ();

		}
		private void Start()
		{ // должен вызывать EndOfTurn

		}

		public void Stop()
		{

		}

	  protected override void PrintType ()
		{
			en.io.ServerWrite ("(Envirotment)" + Environment.NewLine, 14, true, true);
		}
	}

	public class MythosRumor : MythosCard
	{
		public MythosRumor ( GameEngine eng, short pID) : base(eng, pID)
		{

		}


		protected override void Step4 ()
		{ en.io.ServerWrite (Environment.NewLine+ en.sysstr.GetString (SSType.MythosStep4)+Environment.NewLine, 12, true ); 
			if (en.curs.curRumor != null)
			{

				en.mythosDeck.Discard (this);
				en.clock.EndTurn (); 
			}
			en.curs.curRumor = this;
			Init ();

		}
		private void Init()
		{ 

		}

		private void Tick()
		{

		}

		public void Reset()
		{


		}

		public void Pass()
		{

		}

		private void Fail()
		{

		}

		protected override void PrintType ()
		{
			en.io.ServerWrite ("(Rumor)" + Environment.NewLine, 14, true, true);
		}



	}


}

