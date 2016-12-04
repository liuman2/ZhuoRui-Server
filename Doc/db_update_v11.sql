INSERT INTO `menu` VALUES ('72', '15', 'letter_insert', 'fa fa-file-o', '新增寄件', '38');
INSERT INTO `menu` VALUES ('73', '15', 'inbox_insert', 'fa fa-file-o', '新增收件', '39');


call AddColumnUnlessExists(Database(), 'settings', 'memo', 'varchar(50) DEFAULT NULL COMMENT "说明"');

ALTER TABLE mail ADD CONSTRAINT fk_mail_creator_id FOREIGN KEY (creator_id) REFERENCES member(id);
INSERT INTO `settings` VALUES ('4', 'JW_ID', null, null);
INSERT INTO `settings` VALUES ('5', 'GN_ID', null, null);
INSERT INTO `settings` VALUES ('6', 'JWSJ_ID', null, null);
INSERT INTO `settings` VALUES ('7', 'GNSJ_ID', null, null);
INSERT INTO `settings` VALUES ('8', 'SB_ID', null, null);
INSERT INTO `settings` VALUES ('9', 'ZL_ID', null, null);
INSERT INTO `settings` VALUES ('10', 'CW_ID', null, null);


call AddColumnUnlessExists(Database(), 'audit', 'account_period2', 'datetime DEFAULT NULL COMMENT "账期"');
alter table audit modify column date_year_end varchar(6);

call AddColumnUnlessExists(Database(), 'audit', 'source', 'varchar(30) DEFAULT NULL');
call AddColumnUnlessExists(Database(), 'audit', 'source_id', 'int(11) DEFAULT NULL');
call AddColumnUnlessExists(Database(), 'audit', 'source_code', 'varchar(20) DEFAULT NULL COMMENT "来源单号"');



DROP TABLE IF EXISTS `receipt`;
CREATE TABLE `receipt` (
  `id` int(11) NOT NULL,
  `code` varchar(6) DEFAULT NULL COMMENT '流水号',
  `order_source` varchar(20) DEFAULT NULL COMMENT '订单来源',
  `order_id` int(11) DEFAULT NULL COMMENT '订单id',
  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `date_created` datetime DEFAULT NULL,
  `memo` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_receipt_creator_id` (`creator_id`),
  CONSTRAINT `receipt_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `sequence` VALUES ('receipt', '1');
INSERT INTO `dictionary_group` VALUES ('15', '收款事由', '收款事由');




