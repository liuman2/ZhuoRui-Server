﻿ 

using WebCenter.IDAL;
using WebCenter.Entities;

namespace WebCenter.DAL
{
   
	
	public partial class areaRepository :BaseRepository<area>,IareaRepository
    {
         
    }
	
	public partial class bank_accountRepository :BaseRepository<bank_account>,Ibank_accountRepository
    {
         
    }
	
	public partial class customerRepository :BaseRepository<customer>,IcustomerRepository
    {
         
    }
	
	public partial class customer_timelineRepository :BaseRepository<customer_timeline>,Icustomer_timelineRepository
    {
         
    }
	
	public partial class dictionaryRepository :BaseRepository<dictionary>,IdictionaryRepository
    {
         
    }
	
	public partial class dictionary_groupRepository :BaseRepository<dictionary_group>,Idictionary_groupRepository
    {
         
    }
	
	public partial class incomeRepository :BaseRepository<income>,IincomeRepository
    {
         
    }
	
	public partial class memberRepository :BaseRepository<member>,ImemberRepository
    {
         
    }
	
	public partial class organizationRepository :BaseRepository<organization>,IorganizationRepository
    {
         
    }
	
	public partial class positionRepository :BaseRepository<position>,IpositionRepository
    {
         
    }
	
	public partial class reg_abroadRepository :BaseRepository<reg_abroad>,Ireg_abroadRepository
    {
         
    }
	
	public partial class reg_historyRepository :BaseRepository<reg_history>,Ireg_historyRepository
    {
         
    }
	
	public partial class roleRepository :BaseRepository<role>,IroleRepository
    {
         
    }
	
	public partial class sequenceRepository :BaseRepository<sequence>,IsequenceRepository
    {
         
    }
	
	public partial class timelineRepository :BaseRepository<timeline>,ItimelineRepository
    {
         
    }
	
}