call AddColumnUnlessExists(Database(), 'reg_internal', 'names', 'text DEFAULT NULL COMMENT "公司备选名称"');
-- call AddColumnUnlessExists(Database(), 'reg_internal', 'prices', 'text DEFAULT NULL COMMENT "费用记录"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'shareholders', 'text DEFAULT NULL COMMENT "股东"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'card_no', 'varchar(100) DEFAULT NULL COMMENT "身份证号"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'scope', 'text DEFAULT NULL COMMENT "经营范围"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'pay_mode', 'varchar(10) DEFAULT NULL COMMENT "缴交方式"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'capital', ' float(255,2) DEFAULT NULL COMMENT "注册资本"');


INSERT INTO `dictionary_group` VALUES ('15', '委托事项', '委托事项');
INSERT INTO `dictionary` VALUES ('185', '委托事项', '确认注册信息', 1, null, null);
INSERT INTO `dictionary` VALUES ('186', '委托事项', '名称预核算', 1, null, null);
INSERT INTO `dictionary` VALUES ('187', '委托事项', '网上设立申请', 1, null, null);
INSERT INTO `dictionary` VALUES ('188', '委托事项', '办理营业执照', 1, null, null);
INSERT INTO `dictionary` VALUES ('189', '委托事项', '刻制企业印章', 1, null, null);
INSERT INTO `dictionary` VALUES ('190', '委托事项', '开银行基本户', 1, null, null);
INSERT INTO `dictionary` VALUES ('191', '委托事项', '一般纳税人', 1, null, null);
INSERT INTO `dictionary` VALUES ('192', '委托事项', '进出口权', 1, null, null);
INSERT INTO `dictionary` VALUES ('193', '委托事项', '食品经营许可', 1, null, null);
INSERT INTO `dictionary` VALUES ('194', '委托事项', '酒类经营许可', 1, null, null);


DROP TABLE IF EXISTS `reg_internal_items`;
CREATE TABLE `reg_internal_items` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL,
  `name` varchar(50) DEFAULT NULL COMMENT '委托事项',
  `material` varchar(500) DEFAULT NULL COMMENT '所需资料',
  `spend` int DEFAULT NULL COMMENT '办理时间',
  `price` float(255,2) DEFAULT NULL COMMENT '收费标准',
  `memo` varchar(500) DEFAULT NULL COMMENT '备注',
  `finisher` varchar(30) DEFAULT NULL COMMENT '负责人',
  `status` int(11) DEFAULT NULL COMMENT '状态 未完成：0；已完成：1',
  `date_finished` datetime DEFAULT NULL,
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('reg_internal_items', '1');


call AddColumnUnlessExists(Database(), 'reg_internal', 'biz_address', ' varchar(300) DEFAULT NULL COMMENT "经营地址"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'director_card_no', ' varchar(100) DEFAULT NULL COMMENT "监事身份证"');
call AddColumnUnlessExists(Database(), 'reg_internal_items', 'sub_items', ' text DEFAULT NULL COMMENT "子事项"');
