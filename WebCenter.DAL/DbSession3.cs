 
 
using WebCenter.IDAL;

namespace WebCenter.DAL
{
    public partial class DbSession:IDbSession  
    {   
	
	public IDAL.Iannual_examRepository annual_examRepository { get { return new annual_examRepository(); } }
	
	public IDAL.IareaRepository areaRepository { get { return new areaRepository(); } }
	
	public IDAL.IauditRepository auditRepository { get { return new auditRepository(); } }
	
	public IDAL.Ibank_accountRepository bank_accountRepository { get { return new bank_accountRepository(); } }
	
	public IDAL.IcustomerRepository customerRepository { get { return new customerRepository(); } }
	
	public IDAL.Icustomer_timelineRepository customer_timelineRepository { get { return new customer_timelineRepository(); } }
	
	public IDAL.IdictionaryRepository dictionaryRepository { get { return new dictionaryRepository(); } }
	
	public IDAL.Idictionary_groupRepository dictionary_groupRepository { get { return new dictionary_groupRepository(); } }
	
	public IDAL.IincomeRepository incomeRepository { get { return new incomeRepository(); } }
	
	public IDAL.IlectureRepository lectureRepository { get { return new lectureRepository(); } }
	
	public IDAL.Ilecture_customerRepository lecture_customerRepository { get { return new lecture_customerRepository(); } }
	
	public IDAL.ImailRepository mailRepository { get { return new mailRepository(); } }
	
	public IDAL.ImemberRepository memberRepository { get { return new memberRepository(); } }
	
	public IDAL.ImenuRepository menuRepository { get { return new menuRepository(); } }
	
	public IDAL.IoperationRepository operationRepository { get { return new operationRepository(); } }
	
	public IDAL.IorganizationRepository organizationRepository { get { return new organizationRepository(); } }
	
	public IDAL.IpatentRepository patentRepository { get { return new patentRepository(); } }
	
	public IDAL.IpositionRepository positionRepository { get { return new positionRepository(); } }
	
	public IDAL.Ireg_abroadRepository reg_abroadRepository { get { return new reg_abroadRepository(); } }
	
	public IDAL.Ireg_historyRepository reg_historyRepository { get { return new reg_historyRepository(); } }
	
	public IDAL.Ireg_internalRepository reg_internalRepository { get { return new reg_internalRepository(); } }
	
	public IDAL.Ireg_internal_historyRepository reg_internal_historyRepository { get { return new reg_internal_historyRepository(); } }
	
	public IDAL.IroleRepository roleRepository { get { return new roleRepository(); } }
	
	public IDAL.Irole_memberRepository role_memberRepository { get { return new role_memberRepository(); } }
	
	public IDAL.Irole_memuRepository role_memuRepository { get { return new role_memuRepository(); } }
	
	public IDAL.Irole_operationRepository role_operationRepository { get { return new role_operationRepository(); } }
	
	public IDAL.IsequenceRepository sequenceRepository { get { return new sequenceRepository(); } }
	
	public IDAL.IsettingRepository settingRepository { get { return new settingRepository(); } }
	
	public IDAL.ItimelineRepository timelineRepository { get { return new timelineRepository(); } }
	
	public IDAL.ItrademarkRepository trademarkRepository { get { return new trademarkRepository(); } }
	}
}