using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorInfor  {
 	  
  	public  string RoleName=string.Empty;
 	public  string name; 
	 
 	public string RolePicture="inveiteFriend";
	public string RoleAnimationName;
	
 	public int NoSeat=-1;
	public int gamblingNo=-1;
	 
 	public bool HedPokeCard1 =false;
	public bool HedPokeCard2=false;
 	
	public string pokecard1value="--";
	public string pokecard2value="--";
 	
	public bool bigCard=false;
  	
 	
	public int nextOne=-1;
	public bool isplaying=false;
	public bool isAllin=false;
	
	public bool halfAlpaha=false;
	
	
	public string EasyMotionName=string.Empty;
	
	public ActorInfor()
	{
		
  	}
	 
}

public class ActorInforCompare:IEqualityComparer<ActorInfor>{
	
	public bool Equals(ActorInfor x ,ActorInfor y){
		
		if(x.NoSeat==y.NoSeat)
			return true;
		
		return false;
		
	}
	
	public int GetHashCode(ActorInfor x){
		
		return 0;
	}
	
}

 
