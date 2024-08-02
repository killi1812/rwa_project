--TODO mby rename 
CREATE DATABASE rwa;
GO

USE rwa;
GO

CREATE TABLE  users (
                        id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
                        name VARCHAR(255) NOT NULL,
                        admin bit NOT NULL,
                        password VARCHAR(255) NOT NULL
);
-- 1-n relationship between users and pictures
create table pictures (
                          id int identity(1,1) primary key not null,
                          name varchar(255) not null,
                          photographer varchar(255) not null,
                          userId int not null,
                          data varbinary(max) not null,
                          foreign key (userId) references users(id)
);

create table tags (
                      id int identity(1,1) primary key not null,
                      name varchar(255) not null
);

-- m-n relationship between pictures and tags
create table pictureTags (
                             pictureId int not null,
                             tagId int not null,
                             foreign key (pictureId) references pictures(id),
                             foreign key (tagId) references tags(id)
);

-- m-n relationship between users and pictures
create table downloads (
                           id int identity(1,1) primary key not null,
                           date datetime not null,
                           pictureId int not null,
                           userId int not null,
                           foreign key (pictureId) references pictures(id),
                           foreign key (userId) references users(id)
);
GO
