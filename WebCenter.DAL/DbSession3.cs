 
 
using WebCenter.IDAL;

namespace WebCenter.DAL
{
    public partial class DbSession:IDbSession  
    {   
	
	public IDAL.IareaRepository areaRepository { get { return new areaRepository(); } }
	
	public IDAL.Ibank_accountRepository bank_accountRepository { get { return new bank_accountRepository(); } }
	
	public IDAL.IcustomerRepository customerRepository { get { return new customerRepository(); } }
	
	public IDAL.Icustomer_timelineRepository customer_timelineRepository { get { return new customer_timelineRepository(); } }
	
	public IDAL.IdictionaryRepository dictionaryRepository { get { return new dictionaryRepository(); } }
	
	public IDAL.Idictionary_groupRepository dictionary_groupRepository { get { return new dictionary_groupRepository(); } }
	
	public IDAL.IincomeRepository incomeRepository { get { return new incomeRepository(); } }
	
	public IDAL.ImemberRepository memberRepository { get { return new memberRepository(); } }
	
	public IDAL.IorganizationRepository organizationRepository { get { return new organizationRepository(); } }
	
	public IDAL.IpositionRepository positionRepository { get { return new positionRepository(); } }
	
	public IDAL.Ireg_abroadRepository reg_abroadRepository { get { return new reg_abroadRepository(); } }
	
	public IDAL.IroleRepository roleRepository { get { return new roleRepository(); } }
	
	public IDAL.IsequenceRepository sequenceRepository { get { return new sequenceRepository(); } }
	
	public IDAL.ItimelineRepository timelineRepository { get { return new timelineRepository(); } }
	}
}