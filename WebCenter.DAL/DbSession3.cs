﻿ 
 
using WebCenter.IDAL;

namespace WebCenter.DAL
{
    public partial class DbSession:IDbSession  
    {   
	
	public IDAL.Iabroad_historyRepository abroad_historyRepository { get { return new abroad_historyRepository(); } }
	
	public IDAL.Iabroad_shareholderRepository abroad_shareholderRepository { get { return new abroad_shareholderRepository(); } }
	
	public IDAL.IaccountingRepository accountingRepository { get { return new accountingRepository(); } }
	
	public IDAL.Iaccounting_itemRepository accounting_itemRepository { get { return new accounting_itemRepository(); } }
	
	public IDAL.Iaccounting_progressRepository accounting_progressRepository { get { return new accounting_progressRepository(); } }
	
	public IDAL.Iannual_examRepository annual_examRepository { get { return new annual_examRepository(); } }
	
	public IDAL.IareaRepository areaRepository { get { return new areaRepository(); } }
	
	public IDAL.IattachmentRepository attachmentRepository { get { return new attachmentRepository(); } }
	
	public IDAL.IauditRepository auditRepository { get { return new auditRepository(); } }
	
	public IDAL.Iaudit_bankRepository audit_bankRepository { get { return new audit_bankRepository(); } }
	
	public IDAL.IbankRepository bankRepository { get { return new bankRepository(); } }
	
	public IDAL.Ibank_accountRepository bank_accountRepository { get { return new bank_accountRepository(); } }
	
	public IDAL.Ibank_contactRepository bank_contactRepository { get { return new bank_contactRepository(); } }
	
	public IDAL.Ibusiness_bankRepository business_bankRepository { get { return new business_bankRepository(); } }
	
	public IDAL.IcontactRepository contactRepository { get { return new contactRepository(); } }
	
	public IDAL.IcustomerRepository customerRepository { get { return new customerRepository(); } }
	
	public IDAL.Icustomer_timelineRepository customer_timelineRepository { get { return new customer_timelineRepository(); } }
	
	public IDAL.IdictionaryRepository dictionaryRepository { get { return new dictionaryRepository(); } }
	
	public IDAL.Idictionary_groupRepository dictionary_groupRepository { get { return new dictionary_groupRepository(); } }
	
	public IDAL.IhistoryRepository historyRepository { get { return new historyRepository(); } }
	
	public IDAL.Ihistory_shareholderRepository history_shareholderRepository { get { return new history_shareholderRepository(); } }
	
	public IDAL.IincomeRepository incomeRepository { get { return new incomeRepository(); } }
	
	public IDAL.Iinternal_historyRepository internal_historyRepository { get { return new internal_historyRepository(); } }
	
	public IDAL.Iinternal_shareholderRepository internal_shareholderRepository { get { return new internal_shareholderRepository(); } }
	
	public IDAL.IleaveRepository leaveRepository { get { return new leaveRepository(); } }
	
	public IDAL.IlectureRepository lectureRepository { get { return new lectureRepository(); } }
	
	public IDAL.Ilecture_customerRepository lecture_customerRepository { get { return new lecture_customerRepository(); } }
	
	public IDAL.ImailRepository mailRepository { get { return new mailRepository(); } }
	
	public IDAL.ImemberRepository memberRepository { get { return new memberRepository(); } }
	
	public IDAL.ImenuRepository menuRepository { get { return new menuRepository(); } }
	
	public IDAL.InoticeRepository noticeRepository { get { return new noticeRepository(); } }
	
	public IDAL.Iopen_bankRepository open_bankRepository { get { return new open_bankRepository(); } }
	
	public IDAL.IoperationRepository operationRepository { get { return new operationRepository(); } }
	
	public IDAL.IorganizationRepository organizationRepository { get { return new organizationRepository(); } }
	
	public IDAL.IpatentRepository patentRepository { get { return new patentRepository(); } }
	
	public IDAL.IpositionRepository positionRepository { get { return new positionRepository(); } }
	
	public IDAL.IreceiptRepository receiptRepository { get { return new receiptRepository(); } }
	
	public IDAL.Ireg_abroadRepository reg_abroadRepository { get { return new reg_abroadRepository(); } }
	
	public IDAL.Ireg_historyRepository reg_historyRepository { get { return new reg_historyRepository(); } }
	
	public IDAL.Ireg_internalRepository reg_internalRepository { get { return new reg_internalRepository(); } }
	
	public IDAL.Ireg_internal_historyRepository reg_internal_historyRepository { get { return new reg_internal_historyRepository(); } }
	
	public IDAL.Ireg_internal_itemsRepository reg_internal_itemsRepository { get { return new reg_internal_itemsRepository(); } }
	
	public IDAL.IroleRepository roleRepository { get { return new roleRepository(); } }
	
	public IDAL.Irole_memberRepository role_memberRepository { get { return new role_memberRepository(); } }
	
	public IDAL.Irole_memuRepository role_memuRepository { get { return new role_memuRepository(); } }
	
	public IDAL.Irole_operationRepository role_operationRepository { get { return new role_operationRepository(); } }
	
	public IDAL.IscheduleRepository scheduleRepository { get { return new scheduleRepository(); } }
	
	public IDAL.IsequenceRepository sequenceRepository { get { return new sequenceRepository(); } }
	
	public IDAL.IsettingRepository settingRepository { get { return new settingRepository(); } }
	
	public IDAL.Isub_auditRepository sub_auditRepository { get { return new sub_auditRepository(); } }
	
	public IDAL.IsupplierRepository supplierRepository { get { return new supplierRepository(); } }
	
	public IDAL.Itax_recordRepository tax_recordRepository { get { return new tax_recordRepository(); } }
	
	public IDAL.ItimelineRepository timelineRepository { get { return new timelineRepository(); } }
	
	public IDAL.ItrademarkRepository trademarkRepository { get { return new trademarkRepository(); } }
	
	public IDAL.IwaitdealRepository waitdealRepository { get { return new waitdealRepository(); } }
	}
}