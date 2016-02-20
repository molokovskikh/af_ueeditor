create table Farm.OffersStat
(
	Id int unsigned not null auto_increment,
	ProductId int unsigned not null,
	ProducerId int unsigned not null,
	Count int not null default 0,
	primary key(Id),
	constraint FK_Farm_OffersStat_ProductId foreign key (ProductId) references Catalogs.Products(Id) on delete cascade,
	constraint FK_Farm_OffersStat_ProducerId foreign key (ProducerId) references Catalogs.Producers(Id) on delete cascade
);

create procedure Farm.UpdateOffersStat()
begin
	delete from Farm.OffersStat;
	insert into Farm.OffersStat(ProductId, ProducerId, Count)
	select ProductId, CodeFirmCr, count(*)
	from Farm.Core0
	where CodeFirmCr is not null
	group by ProductId, CodeFirmCr;
end;

use Farm;

CREATE DEFINER=`RootDBMS`@`127.0.0.1` EVENT `UpdateOffersStatEvent` ON SCHEDULE EVERY 1 DAY STARTS '2010-02-24 22:30:00' ON COMPLETION PRESERVE ENABLE DO BEGIN
	call Farm.UpdateOffersStat();
END
