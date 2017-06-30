DROP TABLE IF EXISTS `contact`;
CREATE TABLE `contact` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NOT NULL COMMENT '客户ID',
  `name` varchar(120) DEFAULT NULL,
  `mobile` varchar(50) DEFAULT NULL,
  `tel` varchar(50) DEFAULT NULL,
  `position` varchar(50) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `wechat` varchar(50) DEFAULT NULL,
  `QQ` varchar(50) DEFAULT NULL,
  `type` varchar(10) DEFAULT NULL COMMENT 'main, other',
  `memo` varchar(100) DEFAULT NULL,
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,

  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('contact', '1');

call AddColumnUnlessExists(Database(), 'history', 'logoff', 'int(1) DEFAULT NULL COMMENT "是否注销"');
call AddColumnUnlessExists(Database(), 'history', 'logoff_memo', 'varchar(100) DEFAULT NULL COMMENT "注销备注"');

call AddColumnUnlessExists(Database(), 'customer', 'mailling_province', 'varchar(20) DEFAULT NULL COMMENT "邮寄地址省份"');
call AddColumnUnlessExists(Database(), 'customer', 'mailling_city', 'varchar(20) DEFAULT NULL COMMENT "邮寄地址市"');
call AddColumnUnlessExists(Database(), 'customer', 'mailling_county', 'varchar(20) DEFAULT NULL COMMENT "邮寄地址县"');
call AddColumnUnlessExists(Database(), 'customer', 'mailling_address', 'varchar(200) DEFAULT NULL COMMENT "邮寄地址"');
