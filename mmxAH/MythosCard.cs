using System;
using System.Collections.Generic;

namespace mmxAH
{
	public abstract  class MythosCard: Card
	{ protected GameEngine en;
		private short GateLoc, ClueLoc;
		private List<byte> MoveBlack, MoveWhite;
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
		{ //заглушка

			en.clock.EndTurn (); 

		}

		private void Step3()
		{

		}

		protected abstract void Step4();


		public virtual bool  FromTextFile(TextFileParser data, TextFileParser text)
		{ GateLoc  = en.map.GetNumberByCodeName (data.GetToken());
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

		}


		protected override void Step4 ()
		{  eff.Execute( en.clock.EndTurn);  

		}

	}


	public class MythosEnv : MythosCard
	{ public MythosEnv ( GameEngine eng, short pID) : base(eng, pID)
		{

		}


		protected override void Step4 ()
		{ if (en.curs.curEnv != null)
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
	}

	public class MythosRumor : MythosCard
	{
		public MythosRumor ( GameEngine eng, short pID) : base(eng, pID)
		{

		}


		protected override void Step4 ()
		{ if (en.curs.curRumor != null)
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



	}


}

