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
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('contact', '1');

call AddColumnUnlessExists(Database(), 'history', 'logoff', 'int(1) DEFAULT NULL COMMENT "是否注销"');
call AddColumnUnlessExists(Database(), 'history', 'logoff_memo', 'varchar(100) DEFAULT NULL COMMENT "注销备注"');
