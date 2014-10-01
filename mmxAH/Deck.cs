using System;
using System.Collections.Generic; 	

namespace mmxAH
{
	public class Deck<CardType> where CardType: Card
	{ private List<CardType> TopZone,  cards, BottomZone;
	  private List<CardType> discard;
		 private const short  ShaflCount=1000;  
		private bool isShuffleDraw, isDiscardToTheBottom;
		public Deck ( bool pShuffleDraw=false, bool pDiscardToTheBottomZone=false)
		{ cards= new List<CardType>();
	      TopZone = new List<CardType>();
			BottomZone = new List<CardType> ();   
		  discard= new List<CardType>();
			isShuffleDraw=pShuffleDraw; 
			isDiscardToTheBottom = pDiscardToTheBottomZone;  


		}

		public  void Add (CardType c)
		{
			cards.Add(c);
		}

		public void AddToTopZone( CardType c)
		{
			TopZone.Insert (0, c); 
		}

		public void AddToTop( CardType c)
		{
			cards.Insert (0, c); 
		}


		public void PutToTheBottomZone( CardType c)
		{
			BottomZone.Add (c); 
		}

		public void PutToTheBottom( CardType c)
		{
			cards.Add (c); 
		}



		public void Discard (CardType c)
		{ if (isDiscardToTheBottom)
				BottomZone.Add (c);
			else
				discard.Add(c);
		}

		public void Destroy ()
		{
			cards.Clear ();
			TopZone.Clear ();
			BottomZone.Clear (); 
			discard.Clear ();
		}

		public virtual  CardType Draw ()
		{  
			if (isShuffleDraw)
			{ Shuffle();
			}

			CardType c;
			if (TopZone.Count != 0)
			{
				c = TopZone [0];
				TopZone.RemoveAt (0);

			} else if (cards.Count != 0)
			{
				c = cards [0];
				cards.RemoveAt (0);

			} else if (BottomZone.Count != 0)
			{
				c = BottomZone [0];
				BottomZone.RemoveAt (0);
				
			} else
				c = null;
				

		 return c;
		}



		public CardType LookAtAndDiscard ()
		{ CardType c= Draw ();
			discard.Add (c);
			return c;

		}

		public CardType DrawFromBottom ()
		{   CardType c;
			if (BottomZone.Count != 0)
			{
				c = BottomZone [BottomZone.Count - 1];
				BottomZone.RemoveAt (BottomZone.Count - 1);

			} else
			{ c = cards [cards.Count - 1];
				cards.RemoveAt (cards.Count - 1);

			}
			return c;
		}

		public void Shuffle ()
		{
			CardType t;
			int num;
			Random r = new Random (Environment.TickCount);  
			for( short j=0; j< ShaflCount; j++)
				for (short i=0; i< cards.Count; i++) 
				{   num= i+(int) Math.Round (r.NextDouble()*(cards.Count-1-i)); 
					t= cards[i];
					cards[i]=cards[num];
					cards[num]=t;
				}

		}


		public void ReshuffleDiscard ()
		{
			foreach (CardType c in discard)
			{
				cards.Add (c);
			}
			discard.Clear ();
			Shuffle();

		}

		public String PrintAll (bool isNewLine=false)
		{ 
			string res= "==Cards("+(cards.Count+ TopZone.Count+ BottomZone.Count).ToString()+  ")==" + Environment.NewLine;
			foreach (CardType c in TopZone)
			{
				res=res+ c.ToString();
				if(isNewLine)
					res=res+Environment.NewLine; 
			}

			foreach (CardType c in cards)
			{
				res=res+ c.ToString();
				if(isNewLine)
					res=res+Environment.NewLine; 
			}

			foreach (CardType c in BottomZone)
			{
				res=res+ c.ToString();
				if(isNewLine)
					res=res+Environment.NewLine; 
			}

			if( discard.Count==0)
				return res;
			res += Environment.NewLine +" ==Discard("+ discard.Count+")==" + Environment.NewLine;
			foreach (CardType c in discard)
			{
				res=res+ c.ToString();
				if(isNewLine)
					res=res+Environment.NewLine; 
			}
			return res;
		}

		public void ToBin(System.IO.BinaryWriter wr)
		{ 
			Reset ();
			wr.Write (cards.Count);
			foreach (CardType c in cards)
				c.ToBin (wr);

		}


		public void Reset()
		{ 
			foreach (CardType c in TopZone)
				cards.Add (c);
			foreach (CardType c in BottomZone)
				cards.Add (c);
			foreach (CardType c in discard)
				cards.Add (c);

		}


		public void ToSave(System.IO.BinaryWriter wr)
		{ wr.Write (TopZone.Count);
			foreach (CardType c in cards)
				wr.Write (c.GetID()); 


			wr.Write (cards.Count);
			foreach (CardType c in cards)
				wr.Write (c.GetID()); 

			wr.Write (BottomZone .Count);
			foreach (CardType c in  BottomZone)
				wr.Write (c.GetID()); 

			wr.Write (discard  .Count);
			foreach (CardType c in  discard )
				wr.Write (c.GetID()); 



		}


		
		public void FromSave(System.IO.BinaryReader  rd)
		{int cc;
			cc = rd.ReadInt32 (); 
			List<CardType> newList= new List<CardType>();
			for (int i=0; i<cc; i++)
				newList.Add (GetCardById(rd.ReadInt16()));
			TopZone = newList;

			cc = rd.ReadInt32 (); 
		      newList= new List<CardType>();
			for (int i=0; i<cc; i++)
				newList.Add (GetCardById (rd.ReadInt16()));
			cards = newList;

			cc = rd.ReadInt32 (); 
			newList= new List<CardType>();
			for (int i=0; i<cc; i++)
				newList.Add (GetCardById (rd.ReadInt16()));
			BottomZone  = newList;


			cc = rd.ReadInt32 (); 
			newList= new List<CardType>();
			for (int i=0; i<cc; i++)
				newList.Add (GetCardById (rd.ReadInt16()));
			discard  = newList;
		



		}


		public CardType GetCardById( short id)
		{ CardType c;
			if (TopZone.Count != 0)
			{
				for (int i=0; i< TopZone.Count; i++)
				{    
					c = TopZone [i];
					if (c.GetID () == id)
						TopZone.RemoveAt (i);
					return c;
				}

			} else if (cards.Count != 0)
			{
				for (int i=0; i< cards.Count; i++)
				{    
					c = cards [i];
					if (c.GetID () == id)
					{
						cards.RemoveAt (i);
						return c;
					}
				}


			} else if (BottomZone.Count != 0)
			{
				for (int i=0; i< BottomZone.Count; i++)
				{    
					c = BottomZone [i];
					if (c.GetID () == id)
						BottomZone.RemoveAt (i);
					return c;
				}

			} else if (discard.Count != 0)
			{
				for (int i=0; i< discard.Count; i++)
				{    
					c = discard [i];
					if (c.GetID () == id)
						discard.RemoveAt (i);
					return c;
				}


			} 
				

			return null;
		

		}

		public short GetCountOfCards()
		{ return (short) ( TopZone.Count + cards.Count + BottomZone.Count);

		}



		
	}

	public class Card
	{ protected short ID;
		public Card ()
		{
		}

		public override  String ToString ()
		{
			return " Card Class. " + ID;
		}
		public short GetID ()
		{
			return ID;
		}
		public bool isSame( Card other)
		{ return ID==other.GetID();  

		}

		public virtual  void ToBin( System.IO.BinaryWriter wr)
		{

		}
		public virtual void FromBin( System.IO.BinaryReader rd)
		{

		}

		public virtual  void WriteToSave( System.IO.BinaryWriter wr)
		{ wr.Write (ID);

		}
		public virtual void ReadFromSave( System.IO.BinaryReader rd)
		{

		}

	}



}

