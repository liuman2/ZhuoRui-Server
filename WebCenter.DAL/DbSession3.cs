﻿ 
 
using WebCenter.IDAL;

namespace WebCenter.DAL
{
    public partial class DbSession:IDbSession  
    {   
	
	public IDAL.IareaRepository areaRepository { get { return new areaRepository(); } }
	
	public IDAL.IcustomerRepository customerRepository { get { return new customerRepository(); } }
	
	public IDAL.IdictionaryRepository dictionaryRepository { get { return new dictionaryRepository(); } }
	
	public IDAL.Idictionary_groupRepository dictionary_groupRepository { get { return new dictionary_groupRepository(); } }
	
	public IDAL.ImemberRepository memberRepository { get { return new memberRepository(); } }
	
	public IDAL.IorganizationRepository organizationRepository { get { return new organizationRepository(); } }
	
	public IDAL.IpositionRepository positionRepository { get { return new positionRepository(); } }
	
	public IDAL.IroleRepository roleRepository { get { return new roleRepository(); } }
	
	public IDAL.IsequenceRepository sequenceRepository { get { return new sequenceRepository(); } }
	}
}