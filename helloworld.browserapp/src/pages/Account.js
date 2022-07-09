import React, { Component } from 'react';
import Discussion from '../components/Discussion';
import Post from '../components/Post';
import Project from '../components/Project';
import Tag from '../components/Tag';
import TopBanner from '../components/TopBanner/TopBanner';
import { sendJSONRequest } from '../requestFuncs';

import './Account.css';

class Account extends Component {
    state = {
        items: [],
        selectedIndex: 0
    }

    handleChange = (index) => {
        let url = "";

        switch (index) {
            case 0:
                url = "/post/get_all"
                break;
            case 1:
                url = "/discussion/get_all"
                break;
            case 2:
                url = "/project/get_all"
                break;
        }

        sendJSONRequest("GET", url, undefined, this.props.tokens.token, {
            creatorId: this.props.user.id
        }).then(response => {

            console.log(response);
            this.setState({ items: [...response.data] })
        }, error => {
            console.log(error);
            this.props.onError(error.message);
        })
    }

    handleCreatorInfos = (index) => {
        let newItems = this.state.items;
        sendJSONRequest("GET", `/users/get/${newItems[index].creatorId}`, undefined, this.props.tokens.token)
            .then(response => {

                newItems[index].creatorImage = response.data.image;
                newItems[index].creatorName = response.data.userName;
                this.setState({ items: newItems })
            }, error => {
                this.props.onError(error.message);
            });
    }

    renderItems = () => {
        switch (this.state.selectedIndex) {
            case 0:
                return this.state.items.map((item, index) =>
                    <div className="account-item" key={index} >
                        <Post keyProp={index} creatorPic={item.creatorImage} creatorName={item.creatorName}
                            tags={item.tags} images={item.imageUrls} imageHeight={200} imageWidth={280} text={item.content} width="100%"
                            onFirstAppear={this.handleCreatorInfos} />
                    </div>
                )
            case 1:
                return this.state.items.map((item, index) =>    
                    <div className="account-item" key={index} style={{ width: "400px" }}>
                        <Discussion keyProp={index} width={600} onFirstAppear={this.handleCreatorInfos}
                            title={item.title} startMessage={item.startMessage} createdAt={item.createdAt} tags={item.tags} creatorImage={item.creatorImage}
                            lastMessage={item.lastMessage} lastMessageAuthor={item.lastMessageAuthor} lastMessageCreated={item.lastMessageCreated}
                            onFirstAppear={this.handleCreatorInfos} />
                    </div>
                )
            case 2:
                return this.state.items.map((item, index) =>
                    <div className="account-item" key={index} style={{ width: "400px" }} >
                        <Project title={item.title} createdAt={item.createdAt} description={item.description}
                            images={item.images} creatorImage={item.creatorImage} tags={item.tags} width={600} imageHeight={300} imageWidth={500}
                            onReportClick={() => this.setState({ showReportDialog: true })} />
                    </div>
                )
        }
    }

    render() {
        return (
            <div className="page-body">
                <div className="account-profile-section">
                    <div className="account-pic-name">
                        <img src={this.props.user.imageUrl} alt="" width={100} height={100} />
                        <p className="account-name">{this.props.user.userName}</p>
                    </div>
                    <div className="account-description-container">
                        <p className="account-description">Description</p>
                        <p>{this.props.user.description}</p>
                    </div>
                    <div className="account-tags">
                        {
                            this.props.user.tags.map((item, index) =>
                                <Tag key={index} name={item.name} fontSize="20px" />
                            )
                        }
                    </div>
                </div>
                <TopBanner bgColor="#F3F2F2" onSelectionChanged={this.handleChange}>
                    <TopBanner.UnderlineItem name="Posts" selectedBorderColor="#FF9900" textColor="black"/>
                    <TopBanner.UnderlineItem name="Discussions" selectedBorderColor="#FF9900" textColor="black" />
                    <TopBanner.UnderlineItem name="Projects" selectedBorderColor="#FF9900" textColor="black" />
                    <TopBanner.UnderlineItem name="Saved" selectedBorderColor="#FF9900" textColor="black" />
                </TopBanner>
                {
                    <div className="account-items">
                    {
                        this.renderItems()
                    }
                    </div>
                }
            </div>
        )
    }
}

export default Account;